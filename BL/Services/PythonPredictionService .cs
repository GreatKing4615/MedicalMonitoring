using System.Text;
using System.Text.Json;

public class PatientFlowData
{
    public DateTime Date { get; set; }
    public int PatientCount { get; set; }
}

public class EquipmentLoadForecast
{
    public DateTime Date { get; set; }
    public int PredictedPatientCount { get; set; }
    public double LoadPercentage { get; set; }
    public bool IsOverloaded { get; set; }
}


public interface IPythonPredictionService
{
    Task<List<EquipmentLoadForecast>> GetEquipmentLoadPredictionAsync(List<PatientFlowData> historicalData, int horizon, int maxCapacity);
}

public class PythonPredictionService: IPythonPredictionService
{
    private readonly HttpClient _httpClient;

    public PythonPredictionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<EquipmentLoadForecast>> GetEquipmentLoadPredictionAsync(
        List<PatientFlowData> historicalData,
        int horizon,
        int maxCapacity)
    {
        var requestData = new
        {
            historical_data = historicalData.Select(d => new
            {
                Date = d.Date.ToString("yyyy-MM-dd"),
                d.PatientCount
            }).ToList(),
            horizon,
            max_capacity = maxCapacity
        };

        var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("http://localhost:5000/predict", content); // http://ml_service:5000/predict

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var predictions = JsonSerializer.Deserialize<List<EquipmentLoadForecast>>(responseContent, options);

        return predictions;
    }
}

