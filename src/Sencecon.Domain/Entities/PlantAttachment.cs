using Sencecon.Domain.Common;

namespace Sencecon.Domain.Entities;

public class PlantAttachment : BaseEntity
{
    public Guid PlantId { get; set; }
    public Plant Plant { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public int Version { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public Guid UploadedBy { get; set; }
    public DateTimeOffset Created { get; set; }
}
