using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Plants.Queries.GetPlantAttachments;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Plants.Commands.UploadPlantAttachments;

public record PlantAttachmentFile
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }
}

public record UploadPlantAttachmentsCommand : IRequest<IReadOnlyList<PlantAttachmentDto>>
{
    public required Guid PlantId { get; init; }
    public required IReadOnlyList<PlantAttachmentFile> Files { get; init; }
    public string? Title { get; init; }
}

public class UploadPlantAttachmentsCommandHandler : IRequestHandler<UploadPlantAttachmentsCommand, IReadOnlyList<PlantAttachmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UploadPlantAttachmentsCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<PlantAttachmentDto>> Handle(UploadPlantAttachmentsCommand request, CancellationToken cancellationToken)
    {
        var plantExists = await _context.Plants
            .AnyAsync(p => p.Id == request.PlantId, cancellationToken);

        if (!plantExists)
        {
            throw new NotFoundException(nameof(Plant), request.PlantId);
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

        var existingVersionCount = await _context.PlantAttachments
            .Where(a => a.PlantId == request.PlantId && a.Title == title)
            .CountAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var entities = request.Files.Select((file, index) => new PlantAttachment
        {
            PlantId = request.PlantId,
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
            _context.PlantAttachments.Add(entity);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return entities.Select(a => new PlantAttachmentDto
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
