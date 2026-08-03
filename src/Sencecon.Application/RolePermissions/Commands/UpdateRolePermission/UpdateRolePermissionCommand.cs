using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.RolePermissions.Queries.GetRolePermissions;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;

namespace Sencecon.Application.RolePermissions.Commands.UpdateRolePermission;

public record UpdateRolePermissionCommand : IRequest<RolePermissionDto>
{
    public required string Role { get; init; }
    public required string Module { get; init; }
    public required bool CanRead { get; init; }
    public required bool CanWrite { get; init; }
}

public class UpdateRolePermissionCommandValidator : AbstractValidator<UpdateRolePermissionCommand>
{
    public UpdateRolePermissionCommandValidator()
    {
        RuleFor(v => v.Role).Must(r => Enum.GetNames<UserRole>().Contains(r)).WithMessage("Unknown role.");
        RuleFor(v => v.Module).Must(ModuleCatalogue.IsKnown).WithMessage("Unknown module.");
        // Write access without read access doesn't make sense — the frontend
        // gates every write control behind the corresponding read/view being
        // visible in the first place.
        RuleFor(v => v).Must(v => v.CanRead || !v.CanWrite).WithMessage("A role can't have write access without read access.");
    }
}

public class UpdateRolePermissionCommandHandler : IRequestHandler<UpdateRolePermissionCommand, RolePermissionDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateRolePermissionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RolePermissionDto> Handle(UpdateRolePermissionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.RolePermissions
            .FirstOrDefaultAsync(p => p.Role == request.Role && p.Module == request.Module, cancellationToken);

        if (entity is null)
        {
            entity = new RolePermission { Role = request.Role, Module = request.Module, Created = DateTimeOffset.UtcNow };
            _context.RolePermissions.Add(entity);
        }

        entity.CanRead = request.CanRead;
        entity.CanWrite = request.CanWrite;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new RolePermissionDto
        {
            Role = entity.Role,
            Module = entity.Module,
            CanRead = entity.CanRead,
            CanWrite = entity.CanWrite,
        };
    }
}
