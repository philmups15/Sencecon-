using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Projects.Queries.GetProjects;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Projects.Queries.GetProjectById;

public record GetProjectByIdQuery : IRequest<ProjectDto>
{
    public required Guid Id { get; init; }
}

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    private readonly IApplicationDbContext _context;

    public GetProjectByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Project), request.Id);
        }

        return new ProjectDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Customer = entity.Customer,
            Stage = entity.Stage,
            ProjectManager = entity.ProjectManager,
            Budget = entity.Budget,
            Actual = entity.Actual,
            Created = entity.Created
        };
    }
}
