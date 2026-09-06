using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class ProjectBudgetLine : BaseAuditableEntity
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Label { get; set; } = string.Empty;
    public decimal BudgetAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public BomCategory? Category { get; set; }
}
