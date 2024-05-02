using DAL.Entities.Common;

namespace DAL.Entities;

public class Research : IAuditTs
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<DeviceType> RequiredDeviceTypes { get; set; } = new ();
    public DateTime? UpdateTs { get; }
    public DateTime CreateTs { get; }
}
