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

    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }

    public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    public ICollection<PlantAttachment> Attachments { get; set; } = new List<PlantAttachment>();
    public ICollection<CommissioningTestResult> CommissioningTests { get; set; } = new List<CommissioningTestResult>();
    public ICollection<Survey> Surveys { get; set; } = new List<Survey>();
    public ICollection<BomItem> BomItems { get; set; } = new List<BomItem>();
    public ICollection<NonConformity> NonConformities { get; set; } = new List<NonConformity>();
}
