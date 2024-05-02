using DAL.Entities.Common;

namespace DAL.Entities;

public class Device : IAuditTs
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Model { get; set; }
    public DeviceType Type { get; set; }
    public DeviceStatus Status { get; set; }
    public DateTime LastServiceTs { get; set; }
    public DateTime UpdateTs { get; }
    public DateTime CreateTs { get; }
}
