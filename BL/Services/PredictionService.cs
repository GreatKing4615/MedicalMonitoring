using DAL.Entities;
using DAL.Enums;
using DAL.Repositories;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;


namespace BL.Services
{
    public interface IPredictionService
    {
        Task<PatientFlowPrediction> PredictPatientFlowAsync(DeviceType deviceType, int horizon);
    }

    public class PredictionService : IPredictionService
    {
        private readonly IResearchHistoryRepository _researchHistoryRepository;

        public PredictionService(IResearchHistoryRepository researchHistoryRepository)
        {
            _researchHistoryRepository = researchHistoryRepository;
        }

        public async Task<PatientFlowPrediction> PredictPatientFlowAsync(DeviceType deviceType, int horizon)
        {
            // Собираем исторические данные
            var historyData = await _researchHistoryRepository.GetPatientFlowDataAsync(deviceType);

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
