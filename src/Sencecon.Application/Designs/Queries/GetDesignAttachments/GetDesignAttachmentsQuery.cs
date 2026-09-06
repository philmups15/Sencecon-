using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.Designs.Queries.GetDesignAttachments;

public record DesignAttachmentDto
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

public record GetDesignAttachmentsQuery : IRequest<IReadOnlyList<DesignAttachmentDto>>
{
    public required Guid DesignId { get; init; }
}

public class GetDesignAttachmentsQueryHandler : IRequestHandler<GetDesignAttachmentsQuery, IReadOnlyList<DesignAttachmentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDesignAttachmentsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<DesignAttachmentDto>> Handle(GetDesignAttachmentsQuery request, CancellationToken cancellationToken)
    {
        var attachments = await _context.DesignAttachments
            .Where(a => a.DesignId == request.DesignId)
            .OrderByDescending(a => a.Created)
            .ToListAsync(cancellationToken);

        var uploaderIds = attachments.Select(a => a.UploadedBy).Distinct().ToList();
        var uploaderNames = await _context.Users
            .Where(u => uploaderIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.DisplayName, cancellationToken);

        return attachments.Select(a => new DesignAttachmentDto
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
