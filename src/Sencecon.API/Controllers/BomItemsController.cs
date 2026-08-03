using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.BomItems.Commands.CreateBomItem;
using Sencecon.Application.BomItems.Commands.DeleteBomItem;
using Sencecon.Application.BomItems.Commands.UpdateBomItem;
using Sencecon.Application.BomItems.Queries.GetBomItemById;
using Sencecon.Application.BomItems.Queries.GetBomItems;
using Sencecon.Domain.Enums;

namespace Sencecon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BomItemsController : ControllerBase
{
    private readonly ISender _sender;

    public BomItemsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize(Policy = "bomItems-read")]
    [ProducesResponseType(typeof(IReadOnlyList<BomItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BomItemDto>>> GetAll()
    {
        var result = await _sender.Send(new GetBomItemsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "bomItems-read")]
    [ProducesResponseType(typeof(BomItemDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BomItemDto>> GetById(Guid id)
    {
        var result = await _sender.Send(new GetBomItemByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "bomItems-write")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create(CreateBomItemRequest request)
    {
        var id = await _sender.Send(new CreateBomItemCommand
        {
            Component = request.Component,
            Quantity = request.Quantity,
            UnitCost = request.UnitCost,
            Supplier = request.Supplier,
            Status = request.Status
        });

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "bomItems-write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, UpdateBomItemRequest request)
    {
        await _sender.Send(new UpdateBomItemCommand
        {
            Id = id,
            Component = request.Component,
            Quantity = request.Quantity,
            UnitCost = request.UnitCost,
            Supplier = request.Supplier,
            Status = request.Status
        });

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "bomItems-write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteBomItemCommand { Id = id });
        return NoContent();
    }
}

public record CreateBomItemRequest(string Component, int Quantity, decimal UnitCost, string Supplier, BomStatus Status);

public record UpdateBomItemRequest(string Component, int Quantity, decimal UnitCost, string Supplier, BomStatus Status);
