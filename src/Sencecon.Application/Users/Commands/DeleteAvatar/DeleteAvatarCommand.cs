using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Users.Queries.GetUsers;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Users.Commands.DeleteAvatar;

public record DeleteAvatarCommand : IRequest<UserDto>
{
    public required Guid UserId { get; init; }
}

public class DeleteAvatarCommandHandler : IRequestHandler<DeleteAvatarCommand, UserDto>
{
    private readonly IApplicationDbContext _context;

    public DeleteAvatarCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(DeleteAvatarCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.User), request.UserId);
        }

        entity.AvatarContent = null;
        entity.AvatarContentType = null;
        entity.AvatarFileName = null;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDto();
    }
}
