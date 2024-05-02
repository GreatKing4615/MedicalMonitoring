namespace DAL.Entities.Common;

public interface IAuditTs
{
    public DateTime? UpdateTs { get;}
    public DateTime CreateTs { get;}
}
