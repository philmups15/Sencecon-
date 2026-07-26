using Sencecon.Domain.Common;

namespace Sencecon.Domain.Entities;

public class AuditLogEntry : BaseAuditableEntity
{
    public Guid? UserId { get; set; }
    public User? User { get; set; }
    public string Action { get; set; } = string.Empty;
}
