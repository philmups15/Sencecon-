using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Projects.Commands.SetProjectActive;

// Soft-delete: projects are deactivated, never removed. Deactivating a project
// cascades to its plants; reactivating brings them back.
public record SetProjectActiveCommand : IRequest
{
    public required Guid Id { get; init; }
    public required bool Active { get; init; }
}

public class SetProjectActiveCommandHandler : IRequestHandler<SetProjectActiveCommand>
{
    private readonly IApplicationDbContext _context;

    public SetProjectActiveCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(SetProjectActiveCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Project), request.Id);
        }

        var now = DateTimeOffset.UtcNow;

        entity.IsActive = request.Active;
        entity.LastModified = now;

        var plants = await _context.Plants
            .Where(p => p.ProjectId == request.Id)
            .ToListAsync(cancellationToken);

        foreach (var plant in plants)
        {
            plant.IsActive = request.Active;
            plant.LastModified = now;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
