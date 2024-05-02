using DAL.Entities.Common;

namespace DAL.Entities;

public class ServiceHistory : IAuditTs
{
    public int Id { get; set; }
    public Device Device { get; set; }
    public DateTime FixTime { get; set; }
    public MedicalWorker MedicalWorker { get; set; }
    public Failure Failure { get; set; }
    public DateTime UpdateTs { get; }
    public DateTime CreateTs { get; }
}
