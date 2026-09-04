using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.Plants.Queries.GetPlantAttachments;

public record PlantAttachmentDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int Version { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long SizeBytes { get; init; }
    public Guid UploadedBy { get; init; }
    public string UploadedByName { get; init; } = string.Empty;
    public DateTimeOffset Created { get; init; }
}

public record GetPlantAttachmentsQuery : IRequest<IReadOnlyList<PlantAttachmentDto>>
{
    public required Guid PlantId { get; init; }
}

public class GetPlantAttachmentsQueryHandler : IRequestHandler<GetPlantAttachmentsQuery, IReadOnlyList<PlantAttachmentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPlantAttachmentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PlantAttachmentDto>> Handle(GetPlantAttachmentsQuery request, CancellationToken cancellationToken)
    {
        var attachments = await _context.PlantAttachments
            .Where(a => a.PlantId == request.PlantId)
            .OrderByDescending(a => a.Created)
            .ToListAsync(cancellationToken);

        var uploaderIds = attachments.Select(a => a.UploadedBy).Distinct().ToList();
        var uploaderNames = await _context.Users
            .Where(u => uploaderIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.DisplayName, cancellationToken);

        return attachments.Select(a => new PlantAttachmentDto
        {
            Id = a.Id,
            Title = a.Title,
            Version = a.Version,
            FileName = a.FileName,
            ContentType = a.ContentType,
            SizeBytes = a.SizeBytes,
            UploadedBy = a.UploadedBy,
            UploadedByName = uploaderNames.GetValueOrDefault(a.UploadedBy, string.Empty),
            Created = a.Created
        }).ToList();
    }
}
