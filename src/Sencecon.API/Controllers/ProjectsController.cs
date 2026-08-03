using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.Projects.Commands.CreateProject;
using Sencecon.Application.Projects.Commands.DeleteProject;
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
            Code = request.Code,
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
}

public record CreateProjectRequest(string Code, string Name, string Customer, LifecycleStage Stage, string ProjectManager, decimal Budget, decimal Actual);

public record UpdateProjectRequest(string Code, string Name, string Customer, LifecycleStage Stage, string ProjectManager, decimal Budget, decimal Actual);
