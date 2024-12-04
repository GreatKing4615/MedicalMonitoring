
namespace DAL.Entities
{
    public class SimulationResult
    {
        public int Id { get; set; }
        public DateTimeOffset SimulationDate { get; set; }
        public int DeviceId { get; set; }
        public Device Device { get; set; }
        public double LoadPercentage { get; set; }
        public bool IsOverloaded { get; set; }
        public int RecommendedAdditionalUnits { get; set; }
    }
}
