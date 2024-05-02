using DAL.Entities.Common;

namespace DAL.Entities;

public class Failure : IAuditTs
{
    public int Id { get;set; }
    public string ErrorCode { get;set; }
    public DateTime? UpdateTs { get; }
    public DateTime CreateTs { get; }
}
