using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Opportunities.Commands.UpdateOpportunity;

public record UpdateOpportunityCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Customer { get; init; }
    public string Capacity { get; init; } = string.Empty;
    public OpportunityStage Stage { get; init; }
    public string Location { get; init; } = string.Empty;
    public string NextAction { get; init; } = string.Empty;
    public string Owner { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string? Notes { get; init; }
}

public class UpdateOpportunityCommandHandler : IRequestHandler<UpdateOpportunityCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateOpportunityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateOpportunityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Opportunities
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Opportunity), request.Id);
        }

        entity.Code = request.Code;
        entity.Customer = request.Customer;
        entity.Capacity = request.Capacity;
        entity.Stage = request.Stage;
        entity.Location = request.Location;
        entity.NextAction = request.NextAction;
        entity.Owner = request.Owner;
        entity.Value = request.Value;
        entity.Notes = request.Notes;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
