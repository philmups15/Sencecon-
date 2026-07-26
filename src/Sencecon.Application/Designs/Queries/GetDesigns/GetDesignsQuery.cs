using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.Designs.Queries.GetDesigns;

public record GetDesignsQuery : IRequest<IReadOnlyList<DesignDto>>;

public class GetDesignsQueryHandler : IRequestHandler<GetDesignsQuery, IReadOnlyList<DesignDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDesignsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<DesignDto>> Handle(GetDesignsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Designs
            .OrderByDescending(d => d.Created)
            .Select(d => new DesignDto
            {
                Id = d.Id,
                Code = d.Code,
                ProjectName = d.ProjectName,
                Status = d.Status,
                Revision = d.Revision,
                SurveyId = d.SurveyId,
                SurveyCode = d.Survey != null ? d.Survey.Code : null,
                Created = d.Created
            })
            .ToListAsync(cancellationToken);
    }
}
