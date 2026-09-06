using Sencecon.Domain.Common;

namespace Sencecon.Domain.Entities;

public class SurveyPhoto : BaseEntity
{
    public Guid SurveyId { get; set; }
    public Survey Survey { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public int Version { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string? Gps { get; set; }
    public Guid UploadedBy { get; set; }
    public DateTimeOffset Created { get; set; }
}
