using Sencecon.Domain.Common;

namespace Sencecon.Domain.Entities;

public class SurveyObstruction : BaseAuditableEntity
{
    public Guid SurveyId { get; set; }
    public Survey Survey { get; set; } = null!;
    public string Item { get; set; } = string.Empty;
    public string Impact { get; set; } = string.Empty;
}
