namespace DAL.Entities
{
    public class PatientFlowData
    {
        public DateTime Date { get; set; }
        public float PatientCount { get; set; }
    }

    public class PatientFlowPrediction
    {
        public float[] ForecastedPatientCounts { get; set; }
    }
    public class PredictedPatientFlow
    {
        public DateTime Date { get; set; }
        public float PredictedPatientCount { get; set; }
    }
}
