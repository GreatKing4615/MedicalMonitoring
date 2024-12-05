using BL.Dtos;
using DAL.Entities;
using DAL.Enums;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public interface ISimulationService
    {
        Task<List<SimulationResultDto>> RunSimulationAsync(DateTimeOffset fromDate, DateTimeOffset toDate, int minPatientsPerDay, int maxPatientsPerDay);
    }
    public class SimulationService : ISimulationService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IResearchRepository _researchRepository;
        private readonly ISimulationResultRepository _simulationResultRepository;
        private readonly IResearchHistoryRepository _researchHistoryRepository;


        public SimulationService(
            IDeviceRepository deviceRepository,
            IResearchRepository researchRepository,
            ISimulationResultRepository simulationResultRepository,
            IResearchHistoryRepository researchHistoryRepository)
        {
            _deviceRepository = deviceRepository;
            _researchRepository = researchRepository;
            _simulationResultRepository = simulationResultRepository;
            _researchHistoryRepository = researchHistoryRepository;
        }
        public async Task<List<SimulationResultDto>> RunSimulationAsync(
     DateTimeOffset fromDate,
     DateTimeOffset toDate,
     int minPatientsPerDay,
     int maxPatientsPerDay)
        {
            // Проверка входных параметров
            if (minPatientsPerDay < 0 || maxPatientsPerDay < 0)
                throw new ArgumentException("Количество пациентов не может быть отрицательным.");
            if (minPatientsPerDay > maxPatientsPerDay)
                throw new ArgumentException("Минимальное количество пациентов не может быть больше максимального.");

            // Очищаем предыдущие результаты симуляции и историю исследований за указанный период
            await _simulationResultRepository.ClearAllAsync();
            await _researchHistoryRepository.DeleteDataInRangeAsync(fromDate, toDate);

            // Получаем устройства и исследования
            var devices = await _deviceRepository.GetAllDevicesAsync();
            var researches = await _researchRepository.GetAllResearchesAsync();

            // Инициализируем генератор случайных чисел
            var random = new Random();

            // **Устанавливаем дату симуляции один раз**
            var simulationDate = DateTimeOffset.UtcNow;

            // Инициализируем словарь загрузки оборудования
            var deviceLoadDict = devices.ToDictionary(d => d.Id, d => 0.0);

            // Инициализируем словарь для отслеживания занятости устройств
            var deviceSchedules = devices.ToDictionary(d => d.Id, d => new List<(DateTimeOffset start, DateTimeOffset end)>());

            // Инициализируем словарь для отслеживания поломок устройств
            var deviceFailures = devices.ToDictionary(d => d.Id, d => new List<(DateTimeOffset start, DateTimeOffset end)>());

            // Список для хранения сгенерированных историй исследований
            var researchHistories = new List<ResearchHistory>();

            // Предполагаем рабочие часы в день
            const int workDayStartHour = 8;
            const int workDayEndHour = 20;
            const int workingHoursPerDay = workDayEndHour - workDayStartHour;
            const int totalMinutesPerDay = workingHoursPerDay * 60;

            // Расчёт общего количества дней симуляции
            int totalDays = (toDate.Date - fromDate.Date).Days + 1;

            // Симулируем для каждого дня
            for (int dayOffset = 0; dayOffset < totalDays; dayOffset++)
            {
                // Создаем дату для текущего дня
                var date = new DateTimeOffset(fromDate.Date.AddDays(dayOffset), TimeSpan.Zero);

                // Генерируем случайные поломки устройств для текущего дня
                foreach (var device in devices)
                {
                    // 10% шанс того, что устройство выйдет из строя в этот день
                    if (random.NextDouble() < 0.1)
                    {
                        // Генерируем время начала и конца поломки
                        var failureStartHour = random.Next(workDayStartHour, workDayEndHour);
                        var failureDurationHours = random.Next(1, workDayEndHour - failureStartHour + 1);
                        var failureStart = date.AddHours(failureStartHour);
                        var failureEnd = failureStart.AddHours(failureDurationHours);

                        deviceFailures[device.Id].Add((failureStart, failureEnd));
                    }
                }

                // Генерируем случайное количество пациентов в день в заданном диапазоне
                int patientsPerDay = random.Next(minPatientsPerDay, maxPatientsPerDay + 1);

                for (int i = 0; i < patientsPerDay; i++)
                {
                    // Случайно выбираем исследование
                    var research = researches[random.Next(researches.Count)];

                    // Получаем устройства, способные выполнить это исследование
                    var compatibleDevices = devices
                        .Where(d => research.DeviceTypes.Contains(d.Type))
                        .ToList();

                    if (!compatibleDevices.Any())
                        continue; // Нет устройств, способных выполнить это исследование

                    // Перемешиваем список совместимых устройств для равномерного распределения нагрузки
                    compatibleDevices = compatibleDevices.OrderBy(_ => random.Next()).ToList();

                    bool scheduled = false;

                    foreach (var device in compatibleDevices)
                    {
                        var schedule = deviceSchedules[device.Id];
                        var failures = deviceFailures[device.Id];

                        // Рабочие часы: с 8:00 до 20:00
                        var workStart = date.AddHours(workDayStartHour);
                        var workEnd = date.AddHours(workDayEndHour);

                        // Генерируем возможное время начала исследования в рабочих часах
                        var possibleStartTime = workStart.AddMinutes(
                            random.Next(0, (int)(workEnd - workStart).TotalMinutes - (int)research.Duration.TotalMinutes));

                        // Проверяем, не попадает ли время на поломку
                        bool isDuringFailure = failures.Any(f =>
                            f.start < possibleStartTime.Add(research.Duration) && f.end > possibleStartTime);

                        if (isDuringFailure)
                            continue; // Устройство недоступно из-за поломки

                        // Проверяем, не пересекается ли с другими исследованиями на этом устройстве
                        bool hasConflict = schedule.Any(s =>
                            s.start < possibleStartTime.Add(research.Duration) && s.end > possibleStartTime);

                        if (!hasConflict)
                        {
                            // Добавляем исследование в расписание устройства
                            schedule.Add((possibleStartTime, possibleStartTime.Add(research.Duration)));

                            // Добавляем длительность к загрузке устройства
                            deviceLoadDict[device.Id] += research.Duration.TotalMinutes;

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
            }

            // Сохраняем все записи в базе данных
            await _researchHistoryRepository.AddRangeAsync(researchHistories);

            // Рассчитываем общее время симуляции в минутах
            int totalSimulationMinutes = totalDays * totalMinutesPerDay;

            // Рассчитываем процент загрузки и рекомендации
            var simulationResults = new List<SimulationResult>();

            foreach (var device in devices)
            {
                var totalLoadMinutes = deviceLoadDict[device.Id];

                var loadPercentage = (totalLoadMinutes / totalSimulationMinutes) * 100;
                var isOverloaded = loadPercentage > 80; // Считаем перегруженным, если загрузка > 80%
                var recommendedAdditionalUnits = isOverloaded ? 1 : 0;

                var simulationResult = new SimulationResult
                {
                    DeviceId = device.Id,
                    SimulationDate = simulationDate, // Используем одну и ту же дату для всех записей
                    LoadPercentage = loadPercentage,
                    IsOverloaded = isOverloaded,
                    RecommendedAdditionalUnits = recommendedAdditionalUnits
                };

                simulationResults.Add(simulationResult);

                // Сохраняем результат в репозиторий
                await _simulationResultRepository.AddAsync(simulationResult);
            }

            // Маппим результаты в DTO
            var simulationResultDtos = simulationResults.Select(sr => new SimulationResultDto
            {
                DeviceId = sr.DeviceId,
                DeviceModelName = devices.First(d => d.Id == sr.DeviceId).ModelName,
                LoadPercentage = sr.LoadPercentage,
                IsOverloaded = sr.IsOverloaded,
                RecommendedAdditionalUnits = sr.RecommendedAdditionalUnits
            }).ToList();

            return simulationResultDtos;
        }

    }
}
