namespace FiolinOne.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public string? CreatedBy { get; protected set; }
    public string? UpdatedBy { get; protected set; }
}
