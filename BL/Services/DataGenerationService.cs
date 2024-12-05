using DAL.Entities;
using DAL.Repositories;
using System.Collections.Concurrent;
using System.Linq;

public interface IDataGenerationService
{
    Task GenerateSyntheticDataAsync(DateTimeOffset fromDate, DateTimeOffset toDate, int simulationsPerDay);
}


public class DataGenerationService : IDataGenerationService
{
    private readonly IResearchHistoryRepository _researchHistoryRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IResearchRepository _researchRepository;

    public DataGenerationService(
        IResearchHistoryRepository researchHistoryRepository,
        IDeviceRepository deviceRepository,
        IResearchRepository researchRepository)
    {
        _researchHistoryRepository = researchHistoryRepository;
        _deviceRepository = deviceRepository;
        _researchRepository = researchRepository;
    }
    public async Task GenerateSyntheticDataAsync(DateTimeOffset fromDate, DateTimeOffset toDate, int simulationsPerDay)
    {
        // Получаем список устройств и исследований из базы данных
        var devices = await _deviceRepository.GetAllDevicesAsync();
        var researches = await _researchRepository.GetAllResearchesAsync();

        // Удаляем существующие данные за указанный период
        await _researchHistoryRepository.DeleteDataInRangeAsync(fromDate, toDate);

        // Генерируем данные в заданном диапазоне дат
        int totalDays = (toDate.Date - fromDate.Date).Days + 1;

        var researchHistories = new ConcurrentBag<ResearchHistory>();

        // Инициализируем словарь для отслеживания занятости устройств
        var deviceSchedules = devices.ToDictionary(d => d.Id, d => new List<(DateTimeOffset start, DateTimeOffset end)>());

        // Инициализируем словарь для отслеживания поломок устройств
        var deviceFailures = devices.ToDictionary(d => d.Id, d => new List<(DateTimeOffset start, DateTimeOffset end)>());

        Parallel.For(0, totalDays, dayOffset =>
        {
            var random = new Random(Guid.NewGuid().GetHashCode());

            // Создаём дату со смещением ноль
            var date = new DateTimeOffset(fromDate.Date.AddDays(dayOffset), TimeSpan.Zero);

            // Генерируем случайные поломки устройств для текущего дня
            foreach (var device in devices)
            {
                // 10% шанс того, что устройство выйдет из строя в этот день
                if (random.NextDouble() < 0.1)
                {
                    // Генерируем время начала и конца поломки
                    var failureStartHour = random.Next(0, 12);
                    var failureDurationHours = random.Next(1, 12);
                    var failureStart = date.AddHours(failureStartHour);
                    var failureEnd = failureStart.AddHours(failureDurationHours);

                    lock (deviceFailures[device.Id])
                    {
                        deviceFailures[device.Id].Add((failureStart, failureEnd));
                    }
                }
            }

            // Генерируем количество симуляций для этого дня
            int simulations = random.Next(1, simulationsPerDay + 1);

            for (int sim = 0; sim < simulations; sim++)
            {
                // Случайно выбираем исследование
                var research = researches[random.Next(researches.Count)];

                // Получаем устройства, способные выполнить это исследование
                var compatibleDevices = devices.Where(d => research.DeviceTypes.Contains(d.Type)).ToList();

                if (!compatibleDevices.Any())
                    continue; // Нет доступных устройств для этого исследования

                // Пытаемся назначить исследование на доступное устройство
                bool scheduled = false;

                // Перемешиваем список совместимых устройств для равномерного распределения нагрузки
                compatibleDevices = compatibleDevices.OrderBy(_ => random.Next()).ToList();

                foreach (var device in compatibleDevices)
                {
                    var schedule = deviceSchedules[device.Id];
                    var failures = deviceFailures[device.Id];

                    // Рабочие часы: с 8:00 до 20:00
                    var workStart = date.AddHours(8);
                    var workEnd = date.AddHours(20);

                    // Генерируем возможное время начала исследования в рабочих часах
                    var possibleStartTime = workStart.AddMinutes(random.Next(0, (int)(workEnd - workStart).TotalMinutes - (int)research.Duration.TotalMinutes));

                    // Проверяем, не попадает ли время на поломку
                    bool isDuringFailure;
                    lock (failures)
                    {
                        isDuringFailure = failures.Any(f => f.start < possibleStartTime.Add(research.Duration) && f.end > possibleStartTime);
                    }

                    if (isDuringFailure)
                        continue; // Устройство недоступно из-за поломки

                    // Проверяем, не пересекается ли с другими исследованиями на этом устройстве
                    bool hasConflict;
                    lock (schedule)
                    {
                        hasConflict = schedule.Any(s => s.start < possibleStartTime.Add(research.Duration) && s.end > possibleStartTime);

                        if (!hasConflict)
                        {
                            // Добавляем исследование в расписание устройства
                            schedule.Add((possibleStartTime, possibleStartTime.Add(research.Duration)));
                        }
                    }

                    if (!hasConflict)
                    {
                        // Создаём запись в истории исследований
                        var researchHistory = new ResearchHistory
                        {
                            Research = research,
                            Device = device,
                            ResearchDate = possibleStartTime,
                            StartTime = possibleStartTime,
                            EndTime = possibleStartTime.Add(research.Duration)
                        };

                        researchHistories.Add(researchHistory);
                        scheduled = true;
                        break;
                    }
                }

                if (!scheduled)
                {
                    // Не удалось назначить исследование на доступное устройство
                    // Можно добавить логирование или учёт отказов
                    continue;
                }
            }
        });

        // Сохраняем все записи в базе данных
        await _researchHistoryRepository.AddRangeAsync(researchHistories);
    }

}