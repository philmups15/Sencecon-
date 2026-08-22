using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class Survey : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string PlantName { get; set; } = string.Empty;
    public SurveyStatus Status { get; set; }
    public int Progress { get; set; }
    public string Surveyor { get; set; } = string.Empty;
    public DateTimeOffset Date { get; set; }

    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }

    public ICollection<Design> Designs { get; set; } = new List<Design>();
}
