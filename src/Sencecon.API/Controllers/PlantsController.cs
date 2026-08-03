using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.Plants.Commands.CreatePlant;
using Sencecon.Application.Plants.Commands.DeletePlant;
using Sencecon.Application.Plants.Commands.UpdatePlant;
using Sencecon.Application.Plants.Queries.GetPlantById;
using Sencecon.Application.Plants.Queries.GetPlants;
using Sencecon.Domain.Enums;

namespace Sencecon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlantsController : ControllerBase
{
    private readonly ISender _sender;

    public PlantsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize(Policy = "plants-read")]
    [ProducesResponseType(typeof(IReadOnlyList<PlantDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PlantDto>>> GetAll()
    {
        var result = await _sender.Send(new GetPlantsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "plants-read")]
    [ProducesResponseType(typeof(PlantDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PlantDto>> GetById(Guid id)
    {
        var result = await _sender.Send(new GetPlantByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "plants-write")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create(CreatePlantRequest request)
    {
        var id = await _sender.Send(new CreatePlantCommand
        {
            Code = request.Code,
            Name = request.Name,
            Stage = request.Stage,
            Capacity = request.Capacity,
            Equipment = request.Equipment,
            PerformanceRatio = request.PerformanceRatio,
            Health = request.Health
        });

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "plants-write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, UpdatePlantRequest request)
    {
        await _sender.Send(new UpdatePlantCommand
        {
            Id = id,
            Code = request.Code,
            Name = request.Name,
            Stage = request.Stage,
            Capacity = request.Capacity,
            Equipment = request.Equipment,
            PerformanceRatio = request.PerformanceRatio,
            Health = request.Health
        });

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "plants-write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeletePlantCommand { Id = id });
        return NoContent();
    }
}

public record CreatePlantRequest(string Code, string Name, LifecycleStage Stage, string Capacity, string Equipment, double? PerformanceRatio, PlantHealth Health);

public record UpdatePlantRequest(string Code, string Name, LifecycleStage Stage, string Capacity, string Equipment, double? PerformanceRatio, PlantHealth Health);
