using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Users.Queries.GetUsers;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Users.Commands.UploadAvatar;

public record UploadAvatarCommand : IRequest<UserDto>
{
    public required Guid UserId { get; init; }
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }
}

public class UploadAvatarCommandValidator : AbstractValidator<UploadAvatarCommand>
{
    private static readonly string[] AllowedContentTypes = ["image/png", "image/jpeg", "image/webp"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public UploadAvatarCommandValidator()
    {
        RuleFor(v => v.ContentType)
            .Must(ct => AllowedContentTypes.Contains(ct))
            .WithMessage("Profile pictures must be PNG, JPEG, or WebP.");

        RuleFor(v => v.Content.LongLength)
            .LessThanOrEqualTo(MaxFileSizeBytes)
            .WithMessage("Profile pictures must be 5 MB or smaller.");

        RuleFor(v => v.FileName).NotEmpty().MaximumLength(260);
    }
}

public class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, UserDto>
{
    private readonly IApplicationDbContext _context;

    public UploadAvatarCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.User), request.UserId);
        }

        entity.AvatarContent = request.Content;
        entity.AvatarContentType = request.ContentType;
        entity.AvatarFileName = request.FileName;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDto();
    }
}
