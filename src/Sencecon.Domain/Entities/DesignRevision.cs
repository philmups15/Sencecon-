using Sencecon.Domain.Common;

namespace Sencecon.Domain.Entities;

public class DesignRevision : BaseEntity
{
    public Guid DesignId { get; set; }
    public Design Design { get; set; } = null!;
    public string Revision { get; set; } = string.Empty;
    public string? Note { get; set; }
    public Guid? ChangedBy { get; set; }
    public DateTimeOffset Created { get; set; }
}
