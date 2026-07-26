using Sencecon.Domain.Enums;

namespace Sencecon.Application.Opportunities.Queries.GetOpportunities;

public record OpportunityDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Customer { get; init; } = string.Empty;
    public string Capacity { get; init; } = string.Empty;
    public OpportunityStage Stage { get; init; }
    public string Location { get; init; } = string.Empty;
    public string NextAction { get; init; } = string.Empty;
    public string Owner { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public DateTimeOffset Created { get; init; }
}
