using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.NonConformities.Commands.CreateNonConformity;
using Sencecon.Application.NonConformities.Commands.DeleteNonConformity;
using Sencecon.Application.NonConformities.Commands.UpdateNonConformity;
using Sencecon.Application.NonConformities.Queries.GetNonConformities;
using Sencecon.Application.NonConformities.Queries.GetNonConformityById;
using Sencecon.Domain.Enums;

namespace Sencecon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NonConformitiesController : ControllerBase
{
    private readonly ISender _sender;

    public NonConformitiesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<NonConformityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<NonConformityDto>>> GetAll()
    {
        var result = await _sender.Send(new GetNonConformitiesQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(NonConformityDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<NonConformityDto>> GetById(Guid id)
    {
        var result = await _sender.Send(new GetNonConformityByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create(CreateNonConformityRequest request)
    {
        var id = await _sender.Send(new CreateNonConformityCommand
        {
            Code = request.Code,
            Description = request.Description,
            PlantName = request.PlantName,
            Status = request.Status
        });

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, UpdateNonConformityRequest request)
    {
        await _sender.Send(new UpdateNonConformityCommand
        {
            Id = id,
            Code = request.Code,
            Description = request.Description,
            PlantName = request.PlantName,
            Status = request.Status
        });

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteNonConformityCommand { Id = id });
        return NoContent();
    }
}

public record CreateNonConformityRequest(string Code, string Description, string PlantName, NonConformityStatus Status);

public record UpdateNonConformityRequest(string Code, string Description, string PlantName, NonConformityStatus Status);
