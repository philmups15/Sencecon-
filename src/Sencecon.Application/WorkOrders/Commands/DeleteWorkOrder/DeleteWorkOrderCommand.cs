using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.WorkOrders.Commands.DeleteWorkOrder;

public record DeleteWorkOrderCommand : IRequest
{
    public required Guid Id { get; init; }
}

public class DeleteWorkOrderCommandHandler : IRequestHandler<DeleteWorkOrderCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteWorkOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.WorkOrders
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.WorkOrder), request.Id);
        }

        _context.WorkOrders.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
