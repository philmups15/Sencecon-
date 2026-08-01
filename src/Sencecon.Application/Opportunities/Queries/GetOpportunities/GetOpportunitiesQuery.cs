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
                Converted = o.Converted,
                CreatedBy = o.CreatedBy,
                CreatedByName = _context.Users.Where(u => u.Id == o.CreatedBy).Select(u => u.DisplayName).FirstOrDefault() ?? string.Empty,
                Created = o.Created,
                StageData = o.StageData,
                Attachments = o.Attachments
                    .OrderByDescending(a => a.Created)
                    .Select(a => new OpportunityAttachmentDto
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Version = a.Version,
                        FileName = a.FileName,
                        ContentType = a.ContentType,
                        SizeBytes = a.SizeBytes,
                        UploadedBy = a.UploadedBy,
                        UploadedByName = _context.Users.Where(u => u.Id == a.UploadedBy).Select(u => u.DisplayName).FirstOrDefault() ?? string.Empty,
                        Created = a.Created
                    })
                    .ToList(),
                Notes = o.Notes
                    .OrderByDescending(n => n.Created)
                    .Select(n => new OpportunityNoteDto
                    {
                        Id = n.Id,
                        Text = n.Text,
                        CreatedBy = n.CreatedBy,
                        CreatedByName = _context.Users.Where(u => u.Id == n.CreatedBy).Select(u => u.DisplayName).FirstOrDefault() ?? string.Empty,
                        Created = n.Created
                    })
                    .ToList(),
                Activity = o.Activity
                    .OrderByDescending(a => a.Created)
                    .Select(a => new OpportunityActivityDto
                    {
                        Id = a.Id,
                        Type = a.Type,
                        Text = a.Text,
                        UserId = a.UserId,
                        UserName = _context.Users.Where(u => u.Id == a.UserId).Select(u => u.DisplayName).FirstOrDefault() ?? string.Empty,
                        Created = a.Created
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);
    }
}
