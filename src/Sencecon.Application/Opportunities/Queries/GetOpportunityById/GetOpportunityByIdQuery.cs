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
            .FirstOrDefaultAsync(cancellationToken);

        if (dto is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Opportunity), request.Id);
        }

        return dto;
    }
}
