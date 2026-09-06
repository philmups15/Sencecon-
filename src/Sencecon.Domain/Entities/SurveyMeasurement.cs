using Sencecon.Domain.Common;

namespace Sencecon.Domain.Entities;

public class SurveyMeasurement : BaseAuditableEntity
{
    public Guid SurveyId { get; set; }
    public Survey Survey { get; set; } = null!;
    public string Field { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
