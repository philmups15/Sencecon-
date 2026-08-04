using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.Users.Queries.GetUsers;

public record GetUsersQuery : IRequest<IReadOnlyList<UserDto>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        // Inline projection rather than the shared ToDto() mapper — EF Core
        // needs to see the object initializer directly to translate this into
        // a SQL SELECT; it can't do that through an arbitrary method call.
        return await _context.Users
            .OrderBy(u => u.DisplayName)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                DisplayName = u.DisplayName,
                Role = u.Role,
                IsActive = u.IsActive,
                Created = u.Created,
                Username = u.Username,
                PhoneNumber = u.PhoneNumber,
                Address = u.Address,
                JobDescription = u.JobDescription,
                HasAvatar = u.AvatarContent != null && u.AvatarContent.Length > 0
            })
            .ToListAsync(cancellationToken);
    }
}
