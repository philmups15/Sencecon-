using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class Subcontractor : BaseAuditableEntity
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public SubcontractorStatus Status { get; set; } = SubcontractorStatus.Active;
}
