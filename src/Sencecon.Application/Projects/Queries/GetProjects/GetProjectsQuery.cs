using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.Projects.Queries.GetProjects;

public record GetProjectsQuery : IRequest<IReadOnlyList<ProjectDto>>;

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
                Created = p.Created
            })
            .ToListAsync(cancellationToken);
    }
}
