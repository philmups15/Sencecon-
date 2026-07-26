using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class Plant : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public LifecycleStage Stage { get; set; }
    public string Capacity { get; set; } = string.Empty;
    public string Equipment { get; set; } = string.Empty;
    public double? PerformanceRatio { get; set; }
    public PlantHealth Health { get; set; } = PlantHealth.Unknown;

    public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
}
