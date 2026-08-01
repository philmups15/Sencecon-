using Sencecon.Domain.Common;

namespace Sencecon.Domain.Entities;

public class OpportunityActivity : BaseEntity
{
    public Guid OpportunityId { get; set; }
    public Opportunity Opportunity { get; set; } = null!;
    public string Type { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public DateTimeOffset Created { get; set; }
}
