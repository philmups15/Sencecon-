using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.WorkOrders.Commands.CreateWorkOrder;
using Sencecon.Application.WorkOrders.Commands.DeleteWorkOrder;
using Sencecon.Application.WorkOrders.Commands.UpdateWorkOrder;
using Sencecon.Application.WorkOrders.Commands.UploadWorkOrderAttachments;
using Sencecon.Application.WorkOrders.Commands.WorkOrderChildren;
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
    [Authorize(Policy = "workOrders-read")]
    [ProducesResponseType(typeof(IReadOnlyList<WorkOrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<WorkOrderDto>>> GetAll()
    {
        var result = await _sender.Send(new GetWorkOrdersQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "workOrders-read")]
    [ProducesResponseType(typeof(WorkOrderDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkOrderDto>> GetById(Guid id)
    {
        var result = await _sender.Send(new GetWorkOrderByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "workOrders-write")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create(CreateWorkOrderRequest request)
    {
        var id = await _sender.Send(new CreateWorkOrderCommand
        {
            Title = request.Title,
            Type = request.Type,
            Priority = request.Priority,
            Assignee = request.Assignee,
            Status = request.Status,
            DueDate = request.DueDate,
            PlantId = request.PlantId
        });

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "workOrders-write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, UpdateWorkOrderRequest request)
    {
        await _sender.Send(new UpdateWorkOrderCommand
        {
            Id = id,
            Title = request.Title,
            Type = request.Type,
            Priority = request.Priority,
            Assignee = request.Assignee,
            Status = request.Status,
            DueDate = request.DueDate,
            PlantId = request.PlantId
        });

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "workOrders-write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteWorkOrderCommand { Id = id });
        return NoContent();
    }

    // ---- Checklist ----
    [HttpPost("{id:guid}/checklist")]
    [Authorize(Policy = "workOrders-write")]
    public async Task<ActionResult<Guid>> AddChecklistItem(Guid id, ChecklistItemRequest request)
        => await _sender.Send(new AddChecklistItemCommand { WorkOrderId = id, Text = request.Text, Order = request.Order });

    [HttpPut("{id:guid}/checklist/{itemId:guid}")]
    [Authorize(Policy = "workOrders-write")]
    public async Task<IActionResult> UpdateChecklistItem(Guid id, Guid itemId, UpdateChecklistItemRequest request)
    {
        await _sender.Send(new UpdateChecklistItemCommand { Id = itemId, Text = request.Text, IsDone = request.IsDone, Order = request.Order });
        return NoContent();
    }

    [HttpDelete("{id:guid}/checklist/{itemId:guid}")]
    [Authorize(Policy = "workOrders-write")]
    public async Task<IActionResult> DeleteChecklistItem(Guid id, Guid itemId)
    {
        await _sender.Send(new DeleteChecklistItemCommand { Id = itemId });
        return NoContent();
    }

    // ---- Parts ----
    [HttpPost("{id:guid}/parts")]
    [Authorize(Policy = "workOrders-write")]
    public async Task<ActionResult<Guid>> AddPart(Guid id, PartRequest request)
        => await _sender.Send(new AddWorkOrderPartCommand { WorkOrderId = id, PartName = request.PartName, Quantity = request.Quantity, Notes = request.Notes });

    [HttpPut("{id:guid}/parts/{itemId:guid}")]
    [Authorize(Policy = "workOrders-write")]
    public async Task<IActionResult> UpdatePart(Guid id, Guid itemId, PartRequest request)
    {
        await _sender.Send(new UpdateWorkOrderPartCommand { Id = itemId, PartName = request.PartName, Quantity = request.Quantity, Notes = request.Notes });
        return NoContent();
    }

    [HttpDelete("{id:guid}/parts/{itemId:guid}")]
    [Authorize(Policy = "workOrders-write")]
    public async Task<IActionResult> DeletePart(Guid id, Guid itemId)
    {
        await _sender.Send(new DeleteWorkOrderPartCommand { Id = itemId });
        return NoContent();
    }

    // ---- Labour ----
    [HttpPost("{id:guid}/labour")]
    [Authorize(Policy = "workOrders-write")]
    public async Task<ActionResult<Guid>> AddLabour(Guid id, LabourRequest request)
        => await _sender.Send(new AddWorkOrderLabourCommand { WorkOrderId = id, PersonName = request.PersonName, Hours = request.Hours, WorkDate = request.WorkDate, Notes = request.Notes });

    [HttpPut("{id:guid}/labour/{itemId:guid}")]
    [Authorize(Policy = "workOrders-write")]
    public async Task<IActionResult> UpdateLabour(Guid id, Guid itemId, LabourRequest request)
    {
        await _sender.Send(new UpdateWorkOrderLabourCommand { Id = itemId, PersonName = request.PersonName, Hours = request.Hours, WorkDate = request.WorkDate, Notes = request.Notes });
        return NoContent();
    }

    [HttpDelete("{id:guid}/labour/{itemId:guid}")]
    [Authorize(Policy = "workOrders-write")]
    public async Task<IActionResult> DeleteLabour(Guid id, Guid itemId)
    {
        await _sender.Send(new DeleteWorkOrderLabourCommand { Id = itemId });
        return NoContent();
    }

    // ---- Attachments ----
    [HttpPost("{id:guid}/attachments")]
    [Authorize(Policy = "workOrders-write")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(60_000_000)]
    public async Task<ActionResult<IReadOnlyList<WorkOrderAttachmentDto>>> UploadAttachments(Guid id, [FromForm] IFormFileCollection files, [FromForm] string? title)
    {
        var attachmentFiles = new List<WorkOrderAttachmentFile>();
        foreach (var file in files)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            attachmentFiles.Add(new WorkOrderAttachmentFile
            {
                FileName = file.FileName,
                ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                Content = stream.ToArray()
            });
        }
        return Ok(await _sender.Send(new UploadWorkOrderAttachmentsCommand { WorkOrderId = id, Files = attachmentFiles, Title = title }));
    }

    [HttpGet("{id:guid}/attachments/{attachmentId:guid}")]
    [Authorize(Policy = "workOrders-read")]
    public async Task<IActionResult> DownloadAttachment(Guid id, Guid attachmentId)
    {
        var result = await _sender.Send(new GetWorkOrderAttachmentQuery { WorkOrderId = id, AttachmentId = attachmentId });
        return File(result.Content, result.ContentType, result.FileName);
    }
}

public record CreateWorkOrderRequest(string Title, WorkOrderType Type, Priority Priority, string Assignee, WorkOrderStatus Status, DateTimeOffset? DueDate, Guid PlantId);

public record UpdateWorkOrderRequest(string Title, WorkOrderType Type, Priority Priority, string Assignee, WorkOrderStatus Status, DateTimeOffset? DueDate, Guid PlantId);

public record ChecklistItemRequest(string Text, int Order);
public record UpdateChecklistItemRequest(string Text, bool IsDone, int Order);
public record PartRequest(string PartName, int Quantity, string? Notes);
public record LabourRequest(string PersonName, decimal Hours, DateTimeOffset? WorkDate, string? Notes);
