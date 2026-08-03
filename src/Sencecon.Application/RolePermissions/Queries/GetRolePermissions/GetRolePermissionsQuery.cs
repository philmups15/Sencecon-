using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;

namespace Sencecon.Application.RolePermissions.Queries.GetRolePermissions;

public record GetRolePermissionsQuery : IRequest<IReadOnlyList<RolePermissionDto>>;

public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, IReadOnlyList<RolePermissionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRolePermissionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<RolePermissionDto>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
    {
        var rows = await _context.RolePermissions.ToListAsync(cancellationToken);
        var byKey = rows.ToDictionary(r => (r.Role, r.Module));

        var result = new List<RolePermissionDto>();
        foreach (var role in Enum.GetNames<UserRole>())
        {
            foreach (var module in ModuleCatalogue.Keys)
            {
                byKey.TryGetValue((role, module), out var row);
                result.Add(new RolePermissionDto
                {
                    Role = role,
                    Module = module,
                    CanRead = row?.CanRead ?? false,
                    CanWrite = row?.CanWrite ?? false,
                });
            }
        }

        return result;
    }
}
