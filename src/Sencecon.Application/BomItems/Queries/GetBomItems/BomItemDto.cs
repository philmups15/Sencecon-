using Sencecon.Domain.Enums;

namespace Sencecon.Application.BomItems.Queries.GetBomItems;

public record BomItemDto
{
    public Guid Id { get; init; }
    public string Component { get; init; } = string.Empty;
    public BomCategory Category { get; init; }
    public int Quantity { get; init; }
    public decimal UnitCost { get; init; }
    public string Supplier { get; init; } = string.Empty;
    public BomStatus Status { get; init; }
    public Guid? PlantId { get; init; }
    public string? PlantName { get; init; }
    public Guid? ProjectId { get; init; }
    public string? ProjectName { get; init; }
    public DateTimeOffset Created { get; init; }
}
