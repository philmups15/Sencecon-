using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.Projects.Queries.GetProjects;

public record GetProjectsQuery : IRequest<IReadOnlyList<ProjectDto>>
{
    public bool IncludeInactive { get; init; }
}

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, IReadOnlyList<ProjectDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProjectsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Projects
            .Where(p => request.IncludeInactive || p.IsActive)
            .OrderBy(p => p.Code)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Customer = p.Customer,
                Stage = p.Stage,
                ProjectManager = p.ProjectManager,
                Budget = p.Budget,
                Actual = p.Actual,
                IsActive = p.IsActive,
                ScheduledStartDate = p.ScheduledStartDate,
                ScheduledEndDate = p.ScheduledEndDate,
                Created = p.Created
            })
            .ToListAsync(cancellationToken);
    }
}
