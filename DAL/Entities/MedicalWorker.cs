using DAL.Entities.Common;

namespace DAL.Entities;

public class MedicalWorker : IAuditTs
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Specialization Specialization { get; set; }
    public DateTime UpdateTs { get; }
    public DateTime CreateTs { get; }
}
