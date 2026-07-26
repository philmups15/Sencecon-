using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.WorkOrders.Commands.CreateWorkOrder;
using Sencecon.Application.WorkOrders.Commands.DeleteWorkOrder;
using Sencecon.Application.WorkOrders.Commands.UpdateWorkOrder;
using Sencecon.Application.WorkOrders.Queries.GetWorkOrderById;
using Sencecon.Application.WorkOrders.Queries.GetWorkOrders;
using Sencecon.Domain.Enums;

namespace Sencecon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkOrdersController : ControllerBase
{
    private readonly ISender _sender;

    public WorkOrdersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WorkOrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<WorkOrderDto>>> GetAll()
    {
        var result = await _sender.Send(new GetWorkOrdersQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WorkOrderDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkOrderDto>> GetById(Guid id)
    {
        var result = await _sender.Send(new GetWorkOrderByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create(CreateWorkOrderRequest request)
    {
        var id = await _sender.Send(new CreateWorkOrderCommand
        {
            Code = request.Code,
            Title = request.Title,
            Type = request.Type,
            Priority = request.Priority,
            Assignee = request.Assignee,
            Status = request.Status,
            PlantId = request.PlantId
        });

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, UpdateWorkOrderRequest request)
    {
        await _sender.Send(new UpdateWorkOrderCommand
        {
            Id = id,
            Code = request.Code,
            Title = request.Title,
            Type = request.Type,
            Priority = request.Priority,
            Assignee = request.Assignee,
            Status = request.Status,
            PlantId = request.PlantId
        });

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteWorkOrderCommand { Id = id });
        return NoContent();
    }
}

public record CreateWorkOrderRequest(string Code, string Title, WorkOrderType Type, Priority Priority, string Assignee, WorkOrderStatus Status, Guid PlantId);

public record UpdateWorkOrderRequest(string Code, string Title, WorkOrderType Type, Priority Priority, string Assignee, WorkOrderStatus Status, Guid PlantId);
