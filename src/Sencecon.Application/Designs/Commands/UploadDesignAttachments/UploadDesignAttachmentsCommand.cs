using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Designs.Queries.GetDesignAttachments;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Designs.Commands.UploadDesignAttachments;

public record DesignAttachmentFile
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }
}

public record UploadDesignAttachmentsCommand : IRequest<IReadOnlyList<DesignAttachmentDto>>
{
    public required Guid DesignId { get; init; }
    public required IReadOnlyList<DesignAttachmentFile> Files { get; init; }
    public string? Title { get; init; }
}

public class UploadDesignAttachmentsCommandValidator : AbstractValidator<UploadDesignAttachmentsCommand>
{
    private const int MaxFiles = 5;
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    public UploadDesignAttachmentsCommandValidator()
    {
        RuleFor(v => v.Files).NotEmpty().WithMessage("At least one file is required.")
            .Must(f => f.Count <= MaxFiles).WithMessage($"No more than {MaxFiles} files can be uploaded at once.");
        RuleFor(v => v.Title).MaximumLength(200);
        RuleForEach(v => v.Files).ChildRules(file =>
        {
            file.RuleFor(f => f.FileName).NotEmpty().MaximumLength(260);
            file.RuleFor(f => f.Content.LongLength).LessThanOrEqualTo(MaxFileSizeBytes)
                .WithMessage("Each file must be 10 MB or smaller.");
        });
    }
}

public class UploadDesignAttachmentsCommandHandler : IRequestHandler<UploadDesignAttachmentsCommand, IReadOnlyList<DesignAttachmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UploadDesignAttachmentsCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<DesignAttachmentDto>> Handle(UploadDesignAttachmentsCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Designs.AnyAsync(d => d.Id == request.DesignId, cancellationToken))
            throw new NotFoundException(nameof(Design), request.DesignId);

        var currentUserId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var uploadedByName = await _context.Users
            .Where(u => u.Id == currentUserId).Select(u => u.DisplayName)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        var title = string.IsNullOrWhiteSpace(request.Title)
            ? System.IO.Path.GetFileNameWithoutExtension(request.Files[0].FileName)
            : request.Title.Trim();

        var existingVersionCount = await _context.DesignAttachments
            .Where(a => a.DesignId == request.DesignId && a.Title == title)
            .CountAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var entities = request.Files.Select((file, index) => new DesignAttachment
        {
            DesignId = request.DesignId,
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
            _context.DesignAttachments.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entities.Select(a => new DesignAttachmentDto
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
