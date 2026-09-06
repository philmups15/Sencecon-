using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class ProjectTask : BaseAuditableEntity
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public DateTimeOffset? DueDate { get; set; }
    public ProjectTaskStatus Status { get; set; } = ProjectTaskStatus.NotStarted;
}
