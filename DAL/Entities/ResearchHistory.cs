namespace DAL.Entities
{
    public class ResearchHistory
    {
        public int Id { get; set; }
        public Research Research { get; set; }
        public DateTimeOffset ResearchDate { get; set; }
        public Device Device { get; set; }

        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
