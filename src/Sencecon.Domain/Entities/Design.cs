using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class Design : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public DesignStatus Status { get; set; }
    public string Revision { get; set; } = string.Empty;

    // Per-tab specification fields (array / inverter / string / cabling / protection / monitoring).
    public Dictionary<string, Dictionary<string, string>> Specs { get; set; } = new();

    public Guid? SurveyId { get; set; }
    public Survey? Survey { get; set; }

    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }

    public ICollection<DesignAttachment> Attachments { get; set; } = new List<DesignAttachment>();
    public ICollection<DesignRevision> Revisions { get; set; } = new List<DesignRevision>();
}
