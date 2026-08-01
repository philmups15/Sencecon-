using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.Opportunities.Queries.GetOpportunities;

public record GetOpportunitiesQuery : IRequest<IReadOnlyList<OpportunityDto>>;

public class GetOpportunitiesQueryHandler : IRequestHandler<GetOpportunitiesQuery, IReadOnlyList<OpportunityDto>>
{
    private readonly IApplicationDbContext _context;

    public GetOpportunitiesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<OpportunityDto>> Handle(GetOpportunitiesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Opportunities
            .OrderByDescending(o => o.Created)
            .Select(o => new OpportunityDto
            {
                Id = o.Id,
                Code = o.Code,
                Customer = o.Customer,
                Capacity = o.Capacity,
                Stage = o.Stage,
                Location = o.Location,
                NextAction = o.NextAction,
                Owner = o.Owner,
                Value = o.Value,
                SiteVisitDate = o.SiteVisitDate,
                SiteVisitNotes = o.SiteVisitNotes,
                ProposalNotes = o.ProposalNotes,
                NegotiationNotes = o.NegotiationNotes,
                WonDate = o.WonDate,
                Created = o.Created
            })
            .ToListAsync(cancellationToken);
    }
}
