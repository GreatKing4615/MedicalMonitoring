using DAL.Entities;
using DAL.Repositories;
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
        var random = new Random();

        // Получаем список устройств и исследований из базы данных
        var devices = await _deviceRepository.GetAllDevicesAsync();
        var researches = await _researchRepository.GetAllResearchesAsync();

        // Удаляем существующие данные за указанный период
        await _researchHistoryRepository.DeleteDataInRangeAsync(fromDate, toDate);

        // Генерируем данные в заданном диапазоне дат
        int totalDays = (toDate.Date - fromDate.Date).Days + 1;

        for (int dayOffset = 0; dayOffset < totalDays; dayOffset++)
        {
            // Создаем дату со смещением ноль
            var date = new DateTimeOffset(fromDate.Date.AddDays(dayOffset), TimeSpan.Zero);

            // Для каждого дня выполняем заданное количество симуляций
            for (int sim = 0; sim < simulationsPerDay; sim++)
            {
                // Случайно выбираем устройство и исследование
                var device = devices[random.Next(devices.Count)];
                var research = researches[random.Next(researches.Count)];

                // Генерируем случайное время в течение дня
                var time = date.AddHours(random.Next(0, 24)).AddMinutes(random.Next(0, 60));

                // Устанавливаем смещение ноль для времени
                time = time.ToOffset(TimeSpan.Zero);

                // Создаем запись в истории исследований
                var researchHistory = new ResearchHistory
                {
                    Research = research,
                    Device = device,
                    ResearchDate = time
                };

                await _researchHistoryRepository.AddAsync(researchHistory);
            }
        }
    }

}