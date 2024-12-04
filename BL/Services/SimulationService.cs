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
        Task<List<SimulationResultDto>> RunSimulationAsync(int simulationDays, int minPatientsPerDay, int maxPatientsPerDay);
    }
    public class SimulationService : ISimulationService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IResearchRepository _researchRepository;
        private readonly ISimulationResultRepository _simulationResultRepository;

        public SimulationService(
            IDeviceRepository deviceRepository,
            IResearchRepository researchRepository,
            ISimulationResultRepository simulationResultRepository)
        {
            _deviceRepository = deviceRepository;
            _researchRepository = researchRepository;
            _simulationResultRepository = simulationResultRepository;
        }

        public async Task<List<SimulationResultDto>> RunSimulationAsync(int simulationDays, int minPatientsPerDay, int maxPatientsPerDay)
        {
            // Проверка входных параметров
            if (minPatientsPerDay < 0 || maxPatientsPerDay < 0)
                throw new ArgumentException("Количество пациентов не может быть отрицательным.");
            if (minPatientsPerDay > maxPatientsPerDay)
                throw new ArgumentException("Минимальное количество пациентов не может быть больше максимального.");

            // Очищаем предыдущие результаты симуляции
            await _simulationResultRepository.ClearAllAsync();

            // Получаем устройства и исследования
            var devices = await _deviceRepository.GetAllDevicesAsync();
            var researches = await _researchRepository.GetAllResearchesAsync();

            // Инициализируем дату симуляции и генератор случайных чисел
            var simulationDate = DateTimeOffset.UtcNow;
            var random = new Random();

            // Инициализируем словарь загрузки оборудования
            var deviceLoadDict = devices.ToDictionary(d => d.Id, d => 0.0);

            // Предполагаем рабочие часы в день
            const int workingHoursPerDay = 8;
            const int totalMinutesPerDay = workingHoursPerDay * 60;
            int totalSimulationMinutes = simulationDays * totalMinutesPerDay;

            // Симулируем для каждого дня
            for (int day = 0; day < simulationDays; day++)
            {
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

                    if (compatibleDevices.Count == 0)
                        continue; // Нет устройств, способных выполнить это исследование

                    // Выбираем устройство с наименьшей загрузкой
                    var device = compatibleDevices.OrderBy(d => deviceLoadDict[d.Id]).First();

                    // Симулируем время, затраченное на исследование
                    var durationMinutes = research.Duration.TotalMinutes;

                    // Добавляем длительность к загрузке устройства
                    deviceLoadDict[device.Id] += durationMinutes;
                }
            }

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
                    SimulationDate = simulationDate,
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
