using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Users.Queries.GetUsers;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Users.Commands.SetUserStatus;

public record SetUserStatusCommand : IRequest<UserDto>
{
    public required Guid UserId { get; init; }
    public required bool Enabled { get; init; }
    public required Guid RequestingUserId { get; init; }
}

public class SetUserStatusCommandHandler : IRequestHandler<SetUserStatusCommand, UserDto>
{
    private readonly IApplicationDbContext _context;

    public SetUserStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(SetUserStatusCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == request.RequestingUserId)
        {
            throw new ConflictException("You cannot disable or enable your own account. Ask another admin to do it.");
        }

        var entity = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.User), request.UserId);
        }

        entity.IsActive = request.Enabled;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDto();
    }
}
