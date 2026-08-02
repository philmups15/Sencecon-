using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Users.Queries.GetUsers;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Users.Commands.UpdateUserRole;

public record UpdateUserRoleCommand : IRequest<UserDto>
{
    public required Guid UserId { get; init; }
    public required UserRole Role { get; init; }
    public required Guid RequestingUserId { get; init; }
}

public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, UserDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateUserRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == request.RequestingUserId)
        {
            throw new ConflictException("You cannot change your own role. Ask another admin to do it.");
        }

        var entity = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.User), request.UserId);
        }

        entity.Role = request.Role;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new UserDto
        {
            Id = entity.Id,
            Email = entity.Email,
            DisplayName = entity.DisplayName,
            Role = entity.Role,
            Created = entity.Created
        };
    }
}
