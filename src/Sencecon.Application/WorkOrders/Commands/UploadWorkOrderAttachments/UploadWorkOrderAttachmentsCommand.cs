using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.WorkOrders.Commands.UploadWorkOrderAttachments;

public record WorkOrderAttachmentFile
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }
}

public record WorkOrderAttachmentDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int Version { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long SizeBytes { get; init; }
    public string UploadedByName { get; init; } = string.Empty;
    public DateTimeOffset Created { get; init; }
}

public record UploadWorkOrderAttachmentsCommand : IRequest<IReadOnlyList<WorkOrderAttachmentDto>>
{
    public required Guid WorkOrderId { get; init; }
    public required IReadOnlyList<WorkOrderAttachmentFile> Files { get; init; }
    public string? Title { get; init; }
}

public class UploadWorkOrderAttachmentsCommandValidator : AbstractValidator<UploadWorkOrderAttachmentsCommand>
{
    private const int MaxFiles = 5;
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    public UploadWorkOrderAttachmentsCommandValidator()
    {
        RuleFor(v => v.Files).NotEmpty()
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

public class UploadWorkOrderAttachmentsCommandHandler : IRequestHandler<UploadWorkOrderAttachmentsCommand, IReadOnlyList<WorkOrderAttachmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UploadWorkOrderAttachmentsCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<WorkOrderAttachmentDto>> Handle(UploadWorkOrderAttachmentsCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.WorkOrders.AnyAsync(w => w.Id == request.WorkOrderId, cancellationToken))
            throw new NotFoundException(nameof(WorkOrder), request.WorkOrderId);

        var currentUserId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var uploadedByName = await _context.Users
            .Where(u => u.Id == currentUserId).Select(u => u.DisplayName)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        var title = string.IsNullOrWhiteSpace(request.Title)
            ? System.IO.Path.GetFileNameWithoutExtension(request.Files[0].FileName)
            : request.Title.Trim();

        var existingVersionCount = await _context.WorkOrderAttachments
            .Where(a => a.WorkOrderId == request.WorkOrderId && a.Title == title)
            .CountAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var entities = request.Files.Select((file, index) => new WorkOrderAttachment
        {
            WorkOrderId = request.WorkOrderId,
            Title = title,
            Version = existingVersionCount + index + 1,
            FileName = file.FileName,
            ContentType = file.ContentType,
            SizeBytes = file.Content.LongLength,
            Content = file.Content,
            UploadedBy = currentUserId,
            Created = now
        }).ToList();

        foreach (var e in entities) _context.WorkOrderAttachments.Add(e);
        await _context.SaveChangesAsync(cancellationToken);

        return entities.Select(a => new WorkOrderAttachmentDto
        {
            Id = a.Id, Title = a.Title, Version = a.Version, FileName = a.FileName, ContentType = a.ContentType,
            SizeBytes = a.SizeBytes, UploadedByName = uploadedByName, Created = a.Created
        }).ToList();
    }
}

public record WorkOrderAttachmentContent
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }
}

public record GetWorkOrderAttachmentQuery : IRequest<WorkOrderAttachmentContent>
{
    public required Guid WorkOrderId { get; init; }
    public required Guid AttachmentId { get; init; }
}

public class GetWorkOrderAttachmentQueryHandler : IRequestHandler<GetWorkOrderAttachmentQuery, WorkOrderAttachmentContent>
{
    private readonly IApplicationDbContext _context;
    public GetWorkOrderAttachmentQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<WorkOrderAttachmentContent> Handle(GetWorkOrderAttachmentQuery request, CancellationToken cancellationToken)
    {
        var a = await _context.WorkOrderAttachments
            .FirstOrDefaultAsync(x => x.Id == request.AttachmentId && x.WorkOrderId == request.WorkOrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(WorkOrderAttachment), request.AttachmentId);
        return new WorkOrderAttachmentContent { FileName = a.FileName, ContentType = a.ContentType, Content = a.Content };
    }
}
