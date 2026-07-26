using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class Project : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public LifecycleStage Stage { get; set; }
    public string ProjectManager { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public decimal Actual { get; set; }
}
