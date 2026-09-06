using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.Projects.Commands.CreateProject;
using Sencecon.Application.Projects.Commands.DeleteProject;
using Sencecon.Application.Projects.Commands.ProjectChildren;
using Sencecon.Application.Projects.Commands.UpdateProject;
using Sencecon.Application.Projects.Queries.GetProjectById;
using Sencecon.Application.Projects.Queries.GetProjects;
using Sencecon.Domain.Enums;

namespace Sencecon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly ISender _sender;

    public ProjectsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize(Policy = "projects-read")]
    [ProducesResponseType(typeof(IReadOnlyList<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetAll()
    {
        var result = await _sender.Send(new GetProjectsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "projects-read")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id)
    {
        var result = await _sender.Send(new GetProjectByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "projects-write")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create(CreateProjectRequest request)
    {
        var id = await _sender.Send(new CreateProjectCommand
        {
            Name = request.Name,
            Customer = request.Customer,
            Stage = request.Stage,
            ProjectManager = request.ProjectManager,
            Budget = request.Budget,
            Actual = request.Actual
        });

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "projects-write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, UpdateProjectRequest request)
    {
        await _sender.Send(new UpdateProjectCommand
        {
            Id = id,
            Code = request.Code,
            Name = request.Name,
            Customer = request.Customer,
            Stage = request.Stage,
            ProjectManager = request.ProjectManager,
            Budget = request.Budget,
            Actual = request.Actual
        });

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "projects-write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteProjectCommand { Id = id });
        return NoContent();
    }

    // ---- Milestones ----
    [HttpPost("{id:guid}/milestones")]
    [Authorize(Policy = "projects-write")]
    public async Task<ActionResult<Guid>> AddMilestone(Guid id, MilestoneRequest request)
        => await _sender.Send(new AddProjectMilestoneCommand { ProjectId = id, Label = request.Label, State = request.State, Order = request.Order });

    [HttpPut("{id:guid}/milestones/{itemId:guid}")]
    [Authorize(Policy = "projects-write")]
    public async Task<IActionResult> UpdateMilestone(Guid id, Guid itemId, MilestoneRequest request)
    {
        await _sender.Send(new UpdateProjectMilestoneCommand { Id = itemId, Label = request.Label, State = request.State, Order = request.Order });
        return NoContent();
    }

    [HttpDelete("{id:guid}/milestones/{itemId:guid}")]
    [Authorize(Policy = "projects-write")]
    public async Task<IActionResult> DeleteMilestone(Guid id, Guid itemId)
    {
        await _sender.Send(new DeleteProjectMilestoneCommand { Id = itemId });
        return NoContent();
    }

    // ---- Tasks ----
    [HttpPost("{id:guid}/tasks")]
    [Authorize(Policy = "projects-write")]
    public async Task<ActionResult<Guid>> AddTask(Guid id, ProjectTaskRequest request)
        => await _sender.Send(new AddProjectTaskCommand { ProjectId = id, Name = request.Name, Owner = request.Owner, DueDate = request.DueDate, Status = request.Status });

    [HttpPut("{id:guid}/tasks/{itemId:guid}")]
    [Authorize(Policy = "projects-write")]
    public async Task<IActionResult> UpdateTask(Guid id, Guid itemId, ProjectTaskRequest request)
    {
        await _sender.Send(new UpdateProjectTaskCommand { Id = itemId, Name = request.Name, Owner = request.Owner, DueDate = request.DueDate, Status = request.Status });
        return NoContent();
    }

    [HttpDelete("{id:guid}/tasks/{itemId:guid}")]
    [Authorize(Policy = "projects-write")]
    public async Task<IActionResult> DeleteTask(Guid id, Guid itemId)
    {
        await _sender.Send(new DeleteProjectTaskCommand { Id = itemId });
        return NoContent();
    }

    // ---- Subcontractors ----
    [HttpPost("{id:guid}/subcontractors")]
    [Authorize(Policy = "projects-write")]
    public async Task<ActionResult<Guid>> AddSubcontractor(Guid id, SubcontractorRequest request)
        => await _sender.Send(new AddSubcontractorCommand { ProjectId = id, Name = request.Name, Scope = request.Scope, Status = request.Status });

    [HttpPut("{id:guid}/subcontractors/{itemId:guid}")]
    [Authorize(Policy = "projects-write")]
    public async Task<IActionResult> UpdateSubcontractor(Guid id, Guid itemId, SubcontractorRequest request)
    {
        await _sender.Send(new UpdateSubcontractorCommand { Id = itemId, Name = request.Name, Scope = request.Scope, Status = request.Status });
        return NoContent();
    }

    [HttpDelete("{id:guid}/subcontractors/{itemId:guid}")]
    [Authorize(Policy = "projects-write")]
    public async Task<IActionResult> DeleteSubcontractor(Guid id, Guid itemId)
    {
        await _sender.Send(new DeleteSubcontractorCommand { Id = itemId });
        return NoContent();
    }

    // ---- Risks ----
    [HttpPost("{id:guid}/risks")]
    [Authorize(Policy = "projects-write")]
    public async Task<ActionResult<Guid>> AddRisk(Guid id, ProjectRiskRequest request)
        => await _sender.Send(new AddProjectRiskCommand { ProjectId = id, Description = request.Description, Severity = request.Severity, Mitigation = request.Mitigation, Status = request.Status });

    [HttpPut("{id:guid}/risks/{itemId:guid}")]
    [Authorize(Policy = "projects-write")]
    public async Task<IActionResult> UpdateRisk(Guid id, Guid itemId, ProjectRiskRequest request)
    {
        await _sender.Send(new UpdateProjectRiskCommand { Id = itemId, Description = request.Description, Severity = request.Severity, Mitigation = request.Mitigation, Status = request.Status });
        return NoContent();
    }

    [HttpDelete("{id:guid}/risks/{itemId:guid}")]
    [Authorize(Policy = "projects-write")]
    public async Task<IActionResult> DeleteRisk(Guid id, Guid itemId)
    {
        await _sender.Send(new DeleteProjectRiskCommand { Id = itemId });
        return NoContent();
    }

    // ---- Budget lines ----
    [HttpPost("{id:guid}/budget-lines")]
    [Authorize(Policy = "projects-write")]
    public async Task<ActionResult<Guid>> AddBudgetLine(Guid id, BudgetLineRequest request)
        => await _sender.Send(new AddProjectBudgetLineCommand { ProjectId = id, Label = request.Label, BudgetAmount = request.BudgetAmount, ActualAmount = request.ActualAmount, Category = request.Category });

    [HttpPut("{id:guid}/budget-lines/{itemId:guid}")]
    [Authorize(Policy = "projects-write")]
    public async Task<IActionResult> UpdateBudgetLine(Guid id, Guid itemId, BudgetLineRequest request)
    {
        await _sender.Send(new UpdateProjectBudgetLineCommand { Id = itemId, Label = request.Label, BudgetAmount = request.BudgetAmount, ActualAmount = request.ActualAmount, Category = request.Category });
        return NoContent();
    }

    [HttpDelete("{id:guid}/budget-lines/{itemId:guid}")]
    [Authorize(Policy = "projects-write")]
    public async Task<IActionResult> DeleteBudgetLine(Guid id, Guid itemId)
    {
        await _sender.Send(new DeleteProjectBudgetLineCommand { Id = itemId });
        return NoContent();
    }
}

public record CreateProjectRequest(string Name, string Customer, LifecycleStage Stage, string ProjectManager, decimal Budget, decimal Actual);

public record UpdateProjectRequest(string Code, string Name, string Customer, LifecycleStage Stage, string ProjectManager, decimal Budget, decimal Actual);

public record MilestoneRequest(string Label, MilestoneState State, int Order);
public record ProjectTaskRequest(string Name, string Owner, DateTimeOffset? DueDate, ProjectTaskStatus Status);
public record SubcontractorRequest(string Name, string Scope, SubcontractorStatus Status);
public record ProjectRiskRequest(string Description, RiskSeverity Severity, string Mitigation, RiskStatus Status);
public record BudgetLineRequest(string Label, decimal BudgetAmount, decimal ActualAmount, BomCategory? Category);
