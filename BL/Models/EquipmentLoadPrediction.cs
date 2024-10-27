namespace BL.Models
{
    // Models/EquipmentLoadPrediction.cs

    public class EquipmentLoadPrediction
    {
        public List<EquipmentLoadForecast> Forecasts { get; set; }
    }

    public class EquipmentLoadForecast
    {
        public DateTime Date { get; set; }
        public int PredictedPatientCount { get; set; }
        public bool IsOverloaded { get; set; }
    }

}
