using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Opportunities.Commands.DeleteOpportunity;

public record DeleteOpportunityCommand : IRequest
{
    public required Guid Id { get; init; }
}

public class DeleteOpportunityCommandHandler : IRequestHandler<DeleteOpportunityCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteOpportunityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteOpportunityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Opportunities
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Opportunity), request.Id);
        }

        _context.Opportunities.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
