using DAL.Enums;

namespace DAL.Entities
{
    public class Device
    {
        public int Id { get; set; }
        public string ModelName { get; set; }
        public DeviceType Type { get; set; }
        public DeviceStatus Status { get; set; }
        public DateTimeOffset CreateTs { get; set; }
        public DateTimeOffset? UpdateTs { get; set; }
        public DateTimeOffset BeginDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public List<ServiceHistory> ServiceHistory { get; set; } = new();
        public List<ResearchHistory> ResearchHistory { get; set; } = new();
    }
}
