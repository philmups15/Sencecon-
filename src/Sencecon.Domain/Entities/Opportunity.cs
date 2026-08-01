using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class Opportunity : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Capacity { get; set; } = string.Empty;
    public OpportunityStage Stage { get; set; }
    public string Location { get; set; } = string.Empty;
    public string NextAction { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public DateTimeOffset? SiteVisitDate { get; set; }
    public string? SiteVisitNotes { get; set; }
    public string? ProposalNotes { get; set; }
    public string? NegotiationNotes { get; set; }
    public DateTimeOffset? WonDate { get; set; }
}
