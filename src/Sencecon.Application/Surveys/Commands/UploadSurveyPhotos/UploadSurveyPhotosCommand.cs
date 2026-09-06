using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Surveys.Commands.UploadSurveyPhotos;

public record SurveyPhotoFile
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }
}

public record SurveyPhotoDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int Version { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long SizeBytes { get; init; }
    public string? Gps { get; init; }
    public string UploadedByName { get; init; } = string.Empty;
    public DateTimeOffset Created { get; init; }
}

public record UploadSurveyPhotosCommand : IRequest<IReadOnlyList<SurveyPhotoDto>>
{
    public required Guid SurveyId { get; init; }
    public required IReadOnlyList<SurveyPhotoFile> Files { get; init; }
    public string? Title { get; init; }
    public string? Gps { get; init; }
}

public class UploadSurveyPhotosCommandValidator : AbstractValidator<UploadSurveyPhotosCommand>
{
    private const int MaxFiles = 10;
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    public UploadSurveyPhotosCommandValidator()
    {
        RuleFor(v => v.Files).NotEmpty()
            .Must(f => f.Count <= MaxFiles).WithMessage($"No more than {MaxFiles} photos can be uploaded at once.");
        RuleFor(v => v.Title).MaximumLength(200);
        RuleFor(v => v.Gps).MaximumLength(60);
        RuleForEach(v => v.Files).ChildRules(file =>
        {
            file.RuleFor(f => f.FileName).NotEmpty().MaximumLength(260);
            file.RuleFor(f => f.Content.LongLength).LessThanOrEqualTo(MaxFileSizeBytes)
                .WithMessage("Each photo must be 10 MB or smaller.");
        });
    }
}

public class UploadSurveyPhotosCommandHandler : IRequestHandler<UploadSurveyPhotosCommand, IReadOnlyList<SurveyPhotoDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UploadSurveyPhotosCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<SurveyPhotoDto>> Handle(UploadSurveyPhotosCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Surveys.AnyAsync(s => s.Id == request.SurveyId, cancellationToken))
            throw new NotFoundException(nameof(Survey), request.SurveyId);

        var currentUserId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var uploadedByName = await _context.Users
            .Where(u => u.Id == currentUserId).Select(u => u.DisplayName)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        var now = DateTimeOffset.UtcNow;
        var existingCount = await _context.SurveyPhotos.CountAsync(p => p.SurveyId == request.SurveyId, cancellationToken);

        var entities = request.Files.Select((file, index) => new SurveyPhoto
        {
            SurveyId = request.SurveyId,
            Title = string.IsNullOrWhiteSpace(request.Title) ? System.IO.Path.GetFileNameWithoutExtension(file.FileName) : request.Title.Trim(),
            Version = existingCount + index + 1,
            FileName = file.FileName,
            ContentType = file.ContentType,
            SizeBytes = file.Content.LongLength,
            Content = file.Content,
            Gps = request.Gps,
            UploadedBy = currentUserId,
            Created = now
        }).ToList();

        foreach (var e in entities) _context.SurveyPhotos.Add(e);
        await _context.SaveChangesAsync(cancellationToken);

        return entities.Select(p => new SurveyPhotoDto
        {
            Id = p.Id, Title = p.Title, Version = p.Version, FileName = p.FileName, ContentType = p.ContentType,
            SizeBytes = p.SizeBytes, Gps = p.Gps, UploadedByName = uploadedByName, Created = p.Created
        }).ToList();
    }
}

// ---- download + delete ----
public record SurveyPhotoContent
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }
}

public record GetSurveyPhotoQuery : IRequest<SurveyPhotoContent>
{
    public required Guid SurveyId { get; init; }
    public required Guid PhotoId { get; init; }
}

public class GetSurveyPhotoQueryHandler : IRequestHandler<GetSurveyPhotoQuery, SurveyPhotoContent>
{
    private readonly IApplicationDbContext _context;
    public GetSurveyPhotoQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<SurveyPhotoContent> Handle(GetSurveyPhotoQuery request, CancellationToken cancellationToken)
    {
        var photo = await _context.SurveyPhotos
            .FirstOrDefaultAsync(p => p.Id == request.PhotoId && p.SurveyId == request.SurveyId, cancellationToken)
            ?? throw new NotFoundException(nameof(SurveyPhoto), request.PhotoId);
        return new SurveyPhotoContent { FileName = photo.FileName, ContentType = photo.ContentType, Content = photo.Content };
    }
}

public record DeleteSurveyPhotoCommand : IRequest
{
    public required Guid SurveyId { get; init; }
    public required Guid PhotoId { get; init; }
}

public class DeleteSurveyPhotoCommandHandler : IRequestHandler<DeleteSurveyPhotoCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteSurveyPhotoCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteSurveyPhotoCommand request, CancellationToken cancellationToken)
    {
        var photo = await _context.SurveyPhotos
            .FirstOrDefaultAsync(p => p.Id == request.PhotoId && p.SurveyId == request.SurveyId, cancellationToken)
            ?? throw new NotFoundException(nameof(SurveyPhoto), request.PhotoId);
        _context.SurveyPhotos.Remove(photo);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
