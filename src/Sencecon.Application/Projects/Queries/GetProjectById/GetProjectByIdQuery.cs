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
            .Include(p => p.Milestones)
            .Include(p => p.Tasks)
            .Include(p => p.Subcontractors)
            .Include(p => p.Risks)
            .Include(p => p.BudgetLines)
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
            IsActive = entity.IsActive,
            ScheduledStartDate = entity.ScheduledStartDate,
            ScheduledEndDate = entity.ScheduledEndDate,
            Created = entity.Created,
            Milestones = entity.Milestones.OrderBy(m => m.Order).ThenBy(m => m.Created)
                .Select(m => new ProjectMilestoneDto { Id = m.Id, Label = m.Label, State = m.State, Order = m.Order }).ToList(),
            Tasks = entity.Tasks.OrderBy(t => t.DueDate ?? DateTimeOffset.MaxValue).ThenBy(t => t.Created)
                .Select(t => new ProjectTaskDto { Id = t.Id, Name = t.Name, Owner = t.Owner, DueDate = t.DueDate, Status = t.Status }).ToList(),
            Subcontractors = entity.Subcontractors.OrderBy(s => s.Created)
                .Select(s => new SubcontractorDto { Id = s.Id, Name = s.Name, Scope = s.Scope, Status = s.Status }).ToList(),
            Risks = entity.Risks.OrderByDescending(r => r.Severity).ThenBy(r => r.Created)
                .Select(r => new ProjectRiskDto { Id = r.Id, Description = r.Description, Severity = r.Severity, Mitigation = r.Mitigation, Status = r.Status }).ToList(),
            BudgetLines = entity.BudgetLines.OrderBy(b => b.Created)
                .Select(b => new ProjectBudgetLineDto { Id = b.Id, Label = b.Label, BudgetAmount = b.BudgetAmount, ActualAmount = b.ActualAmount, Category = b.Category }).ToList()
        };
    }
}
