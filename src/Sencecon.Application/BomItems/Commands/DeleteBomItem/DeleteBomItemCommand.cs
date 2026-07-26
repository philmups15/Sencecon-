using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.BomItems.Commands.DeleteBomItem;

public record DeleteBomItemCommand : IRequest
{
    public required Guid Id { get; init; }
}

public class DeleteBomItemCommandHandler : IRequestHandler<DeleteBomItemCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteBomItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteBomItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.BomItems
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.BomItem), request.Id);
        }

        _context.BomItems.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
