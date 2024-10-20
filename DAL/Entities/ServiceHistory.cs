using DAL.Enums;

namespace DAL.Entities
{
    public class ServiceHistory
    {
        public int Id { get; set; }
        public Device Device { get; set; }
        public DateTimeOffset ServiceDate { get; set; }
        public WorkType WorkType { get; set; }
        public User Responsible { get; set; }
    }
}
