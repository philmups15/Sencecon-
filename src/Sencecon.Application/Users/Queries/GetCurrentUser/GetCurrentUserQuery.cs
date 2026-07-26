using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Users.Queries.GetUsers;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<UserDto>
{
    public required Guid UserId { get; init; }
}

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    private readonly IApplicationDbContext _context;

    public GetCurrentUserQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.User), request.UserId);
        }

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
