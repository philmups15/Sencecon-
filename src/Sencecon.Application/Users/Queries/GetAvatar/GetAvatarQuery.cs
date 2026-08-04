using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Users.Queries.GetAvatar;

public record GetAvatarQuery : IRequest<AvatarContent>
{
    public required Guid UserId { get; init; }
}

public record AvatarContent
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }
}

public class GetAvatarQueryHandler : IRequestHandler<GetAvatarQuery, AvatarContent>
{
    private readonly IApplicationDbContext _context;

    public GetAvatarQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AvatarContent> Handle(GetAvatarQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user?.AvatarContent is null || user.AvatarContent.Length == 0)
        {
            throw new NotFoundException(nameof(Domain.Entities.User), request.UserId);
        }

        return new AvatarContent
        {
            FileName = user.AvatarFileName ?? "avatar",
            ContentType = user.AvatarContentType ?? "application/octet-stream",
            Content = user.AvatarContent
        };
    }
}
