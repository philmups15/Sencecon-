using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class BomItem : BaseAuditableEntity
{
    public string Component { get; set; } = string.Empty;
    public BomCategory Category { get; set; } = BomCategory.Other;
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public BomStatus Status { get; set; }

    public Guid? PlantId { get; set; }
    public Plant? Plant { get; set; }

    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }
}
