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
                Notes = o.Notes,
                CreatedBy = o.CreatedBy,
                CreatedByName = _context.Users.Where(u => u.Id == o.CreatedBy).Select(u => u.DisplayName).FirstOrDefault() ?? string.Empty,
                Created = o.Created,
                Attachments = o.Attachments
                    .OrderByDescending(a => a.Created)
                    .Select(a => new OpportunityAttachmentDto
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        ContentType = a.ContentType,
                        SizeBytes = a.SizeBytes,
                        UploadedBy = a.UploadedBy,
                        UploadedByName = _context.Users.Where(u => u.Id == a.UploadedBy).Select(u => u.DisplayName).FirstOrDefault() ?? string.Empty,
                        Created = a.Created
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);
    }
}
