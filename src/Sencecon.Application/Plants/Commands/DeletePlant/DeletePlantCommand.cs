using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Plants.Commands.DeletePlant;

public record DeletePlantCommand : IRequest
{
    public required Guid Id { get; init; }
}

public class DeletePlantCommandHandler : IRequestHandler<DeletePlantCommand>
{
    private readonly IApplicationDbContext _context;

    public DeletePlantCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeletePlantCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Plants
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Plant), request.Id);
        }

        _context.Plants.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
