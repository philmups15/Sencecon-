using Sencecon.Domain.Enums;

namespace Sencecon.Application.NonConformities.Queries.GetNonConformities;

public record NonConformityDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string PlantName { get; init; } = string.Empty;
    public NonConformityStatus Status { get; init; }
    public DateTimeOffset Created { get; init; }
}
