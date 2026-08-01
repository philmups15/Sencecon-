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
        var dto = await _context.Opportunities
            .Where(o => o.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (dto is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Opportunity), request.Id);
        }

        return dto;
    }
}
