using DAL.Enums;
using DAL.Repositories;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using Accord.Math;


namespace BL.Services
{
    public interface IPredictionService
    {
        Task<PatientFlowPrediction> PredictPatientFlowAsync(DeviceType deviceType, int horizon, DateTimeOffset? fromDate = null);
        Task<List<EquipmentLoadForecast>> PredictEquipmentLoadWithArimaAsync(DeviceType deviceType, int horizon, DateTimeOffset? fromDate = null);
    }

    public class PredictionService : IPredictionService
    {
        private readonly IResearchHistoryRepository _researchHistoryRepository;
        private readonly IPythonPredictionService _pythonPredictionService;


        public PredictionService(IResearchHistoryRepository researchHistoryRepository, IPythonPredictionService pythonPredictionService)
        {
            _researchHistoryRepository = researchHistoryRepository;
            _pythonPredictionService = pythonPredictionService;
        }

        public async Task<PatientFlowPrediction> PredictPatientFlowAsync(DeviceType deviceType, int horizon, DateTimeOffset? fromDate = null)
        {
            // Собираем исторические данные начиная с fromDate
            var historyData = await _researchHistoryRepository.GetPatientFlowDataAsync(deviceType, fromDate);

            // Проверяем наличие достаточного количества данных
            if (historyData.Count < 10)
            {
                throw new InvalidOperationException("Недостаточно данных для прогнозирования.");
            }

            // Преобразуем данные в формат ML.NET
            var mlContext = new MLContext();
            var dataView = mlContext.Data.LoadFromEnumerable(historyData.OrderBy(d => d.Date));

            // Настраиваем модель временных рядов
            var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedPatientCounts",
                inputColumnName: nameof(PatientFlowData.PatientCount),
                windowSize: 7,
                seriesLength: historyData.Count,
                trainSize: historyData.Count,
                horizon: horizon);

            // Обучаем модель
            var model = forecastingPipeline.Fit(dataView);

            // Создаем прогноз
            var forecastingEngine = model.CreateTimeSeriesEngine<PatientFlowData, PatientFlowPredictionInternal>(mlContext);
            var prediction = forecastingEngine.Predict();

            // Получаем дату последнего исторического значения
            var lastDate = historyData.Max(d => d.Date);

            // Формируем список прогнозов с датами
            var forecasts = new List<PredictedPatientFlow>();
            for (int i = 0; i < horizon; i++)
            {
                forecasts.Add(new PredictedPatientFlow
                {
                    Date = lastDate.AddDays(i + 1),
                    PredictedPatientCount = prediction.ForecastedPatientCounts[i]
                });
            }

            return new PatientFlowPrediction { Forecasts = forecasts };
        }


        public async Task<List<EquipmentLoadForecast>> PredictEquipmentLoadWithArimaAsync(
            DeviceType deviceType,
            int horizon,
            DateTimeOffset? fromDate = null)
        {
            // Получаем исторические данные о потоке пациентов
            var historyData = await _researchHistoryRepository.GetPatientFlowDataAsync(deviceType, fromDate);

            if (historyData.Count < 10)
            {
                throw new InvalidOperationException("Недостаточно данных для прогнозирования.");
            }

            // Преобразуем данные в формат, необходимый для Python-сервиса
            var patientFlowData = historyData
                .Select(d => new PatientFlowData
                {
                    Date = d.Date,
                    PatientCount = (int)d.PatientCount
                })
                .ToList();

            // Рассчитываем MaxDailyCapacity на основе исторических данных
            int maxCapacity = await CalculateMaxDailyCapacityAsync(deviceType, fromDate);

            // Вызываем Python-сервис для получения прогноза
            var predictions = await _pythonPredictionService.GetEquipmentLoadPredictionAsync(
                patientFlowData,
                horizon,
                maxCapacity);

            return predictions;
        }

        private async Task<int> CalculateMaxDailyCapacityAsync(DeviceType deviceType, DateTimeOffset? fromDate = null)
        {
            // Получаем исторические данные об использовании оборудования данного типа
            var usageData = await _researchHistoryRepository.GetPatientFlowDataAsync(deviceType, fromDate);

            if (!usageData.Any())
            {
                throw new InvalidOperationException("Недостаточно данных для расчета максимальной пропускной способности.");
            }

            // Группируем данные по дате и суммируем количество пациентов
            var dailyUsageCounts = usageData
                .GroupBy(d => d.Date.Date)
                .Select(g => g.Sum(d => d.PatientCount))
                .ToList();

            // Сортируем и находим 95-й процентиль
            dailyUsageCounts.Sort();
            int index = (int)(0.95 * dailyUsageCounts.Count);
            int percentile95 = (int)dailyUsageCounts[Math.Min(index, dailyUsageCounts.Count - 1)];

            return percentile95;
        }



        // Внутренний класс для предсказаний ML.NET
        private class PatientFlowPredictionInternal
        {
            public float[] ForecastedPatientCounts { get; set; }
        }
    }

    // Классы для возвращаемого результата
    public class PatientFlowPrediction
    {
        public List<PredictedPatientFlow> Forecasts { get; set; }
    }

    public class PredictedPatientFlow
    {
        public DateTime Date { get; set; }
        public float PredictedPatientCount { get; set; }
    }
}
