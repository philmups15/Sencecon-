using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Opportunities.Queries.GetOpportunities;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Opportunities.Commands.UploadOpportunityAttachments;

public record AttachmentFile
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }
}

public record UploadOpportunityAttachmentsCommand : IRequest<IReadOnlyList<OpportunityAttachmentDto>>
{
    public required Guid OpportunityId { get; init; }
    public required IReadOnlyList<AttachmentFile> Files { get; init; }
    public string? Title { get; init; }
}

public class UploadOpportunityAttachmentsCommandHandler : IRequestHandler<UploadOpportunityAttachmentsCommand, IReadOnlyList<OpportunityAttachmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UploadOpportunityAttachmentsCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<OpportunityAttachmentDto>> Handle(UploadOpportunityAttachmentsCommand request, CancellationToken cancellationToken)
    {
        var opportunityExists = await _context.Opportunities
            .AnyAsync(o => o.Id == request.OpportunityId, cancellationToken);

        if (!opportunityExists)
        {
            throw new NotFoundException(nameof(Opportunity), request.OpportunityId);
        }

        var currentUserId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var uploadedByName = await _context.Users
            .Where(u => u.Id == currentUserId)
            .Select(u => u.DisplayName)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        var title = string.IsNullOrWhiteSpace(request.Title)
            ? System.IO.Path.GetFileNameWithoutExtension(request.Files[0].FileName)
            : request.Title.Trim();

        var existingVersionCount = await _context.OpportunityAttachments
            .Where(a => a.OpportunityId == request.OpportunityId && a.Title == title)
            .CountAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var entities = request.Files.Select((file, index) => new OpportunityAttachment
        {
            OpportunityId = request.OpportunityId,
            Title = title,
            Version = existingVersionCount + index + 1,
            FileName = file.FileName,
            ContentType = file.ContentType,
            SizeBytes = file.Content.LongLength,
            Content = file.Content,
            UploadedBy = currentUserId,
            Created = now
        }).ToList();

        foreach (var entity in entities)
        {
            _context.OpportunityAttachments.Add(entity);
            OpportunityActivityLogger.Log(_context, request.OpportunityId, "document", $"Uploaded {title} v{entity.Version}", currentUserId);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return entities.Select(a => new OpportunityAttachmentDto
        {
            Id = a.Id,
            Title = a.Title,
            Version = a.Version,
            FileName = a.FileName,
            ContentType = a.ContentType,
            SizeBytes = a.SizeBytes,
            UploadedBy = a.UploadedBy,
            UploadedByName = uploadedByName,
            Created = a.Created
        }).ToList();
    }
}
