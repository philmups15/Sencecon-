using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class Design : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public DesignStatus Status { get; set; }
    public string Revision { get; set; } = string.Empty;

    public Guid? SurveyId { get; set; }
    public Survey? Survey { get; set; }
}
