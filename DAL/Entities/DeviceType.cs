using DAL.Entities.Common;

namespace DAL.Entities;

public class DeviceType: IAuditTs
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Device> Devices { get; set; } = new ();
    public List<Research> Researches { get; set; } = new ();
    public DateTime? UpdateTs { get; }
    public DateTime CreateTs { get; }
}
