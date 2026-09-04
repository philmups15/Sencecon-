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
                ProjectName = d.Project != null
                    ? d.Project.Name
                    : (d.Survey != null && d.Survey.Project != null ? d.Survey.Project.Name : d.ProjectName),
                Status = d.Status,
                Revision = d.Revision,
                SurveyId = d.SurveyId,
                SurveyCode = d.Survey != null ? d.Survey.Code : null,
                ProjectId = d.ProjectId ?? (d.Survey != null ? d.Survey.ProjectId : null),
                Created = d.Created
            })
            .ToListAsync(cancellationToken);
    }
}
