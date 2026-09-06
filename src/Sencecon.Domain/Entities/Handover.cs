using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class Handover : BaseAuditableEntity
{
    public Guid PlantId { get; set; }
    public Plant Plant { get; set; } = null!;
    public HandoverStatus Status { get; set; } = HandoverStatus.Draft;
    public Guid? SignedOffBy { get; set; }
    public DateTimeOffset? SignedOffDate { get; set; }
    public DateTimeOffset? AcceptanceDate { get; set; }
    public string? Notes { get; set; }

    public string? CertificateFileName { get; set; }
    public string? CertificateContentType { get; set; }
    public byte[]? CertificateContent { get; set; }
}
