using DAL.Enums;

namespace DAL.Entities
{
    public class Research
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<DeviceType> DeviceTypes { get; set; }
        public TimeSpan Duration { get; set; }
        public List<ResearchHistory> History { get; set; } = new();
    }
}
