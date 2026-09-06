using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Plants.Commands.SetPlantActive;

// Soft-delete: plants are deactivated, never removed.
public record SetPlantActiveCommand : IRequest
{
    public required Guid Id { get; init; }
    public required bool Active { get; init; }
}

public class SetPlantActiveCommandHandler : IRequestHandler<SetPlantActiveCommand>
{
    private readonly IApplicationDbContext _context;

    public SetPlantActiveCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(SetPlantActiveCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Plants
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Plant), request.Id);
        }

        entity.IsActive = request.Active;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
