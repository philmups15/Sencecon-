using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Opportunities.Queries.GetOpportunities;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Opportunities.Queries.GetOpportunityById;

public record GetOpportunityByIdQuery : IRequest<OpportunityDto>
{
    public required Guid Id { get; init; }
}

public class GetOpportunityByIdQueryHandler : IRequestHandler<GetOpportunityByIdQuery, OpportunityDto>
{
    private readonly IApplicationDbContext _context;

    public GetOpportunityByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OpportunityDto> Handle(GetOpportunityByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Opportunities
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Opportunity), request.Id);
        }

        return new OpportunityDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Customer = entity.Customer,
            Capacity = entity.Capacity,
            Stage = entity.Stage,
            Location = entity.Location,
            NextAction = entity.NextAction,
            Owner = entity.Owner,
            Value = entity.Value,
            Created = entity.Created
        };
    }
}
