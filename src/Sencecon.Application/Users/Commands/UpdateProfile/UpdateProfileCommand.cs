using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Users.Queries.GetUsers;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Users.Commands.UpdateProfile;

public record UpdateProfileCommand : IRequest<UserDto>
{
    public required Guid UserId { get; init; }
    public required string DisplayName { get; init; }
}

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateProfileCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.User), request.UserId);
        }

        entity.DisplayName = request.DisplayName;
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
