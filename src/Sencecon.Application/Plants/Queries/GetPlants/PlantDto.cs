using Sencecon.Domain.Enums;

namespace Sencecon.Application.Plants.Queries.GetPlants;

public record PlantDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public LifecycleStage Stage { get; init; }
    public PlantType Type { get; init; }
    public string Capacity { get; init; } = string.Empty;
    public string Equipment { get; init; } = string.Empty;
    public double? PerformanceRatio { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public PlantHealth Health { get; init; }
    public bool IsActive { get; init; } = true;
    public Guid? ProjectId { get; init; }
    public string? ProjectName { get; init; }
    public DateTimeOffset Created { get; init; }
}
