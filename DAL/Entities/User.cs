using DAL.Enums;

namespace DAL.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public Role Role { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset DeleteDate { get; set;}
        public List<ServiceHistory> History { get; set; }
    }
}
