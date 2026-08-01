using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.Opportunities.Commands.CreateOpportunity;
using Sencecon.Application.Opportunities.Commands.DeleteOpportunity;
using Sencecon.Application.Opportunities.Commands.UpdateOpportunity;
using Sencecon.Application.Opportunities.Commands.UploadOpportunityAttachments;
using Sencecon.Application.Opportunities.Queries.GetOpportunities;
using Sencecon.Application.Opportunities.Queries.GetOpportunityAttachment;
using Sencecon.Application.Opportunities.Queries.GetOpportunityById;
using Sencecon.Domain.Enums;

namespace Sencecon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OpportunitiesController : ControllerBase
{
    private readonly ISender _sender;

    public OpportunitiesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OpportunityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OpportunityDto>>> GetAll()
    {
        var result = await _sender.Send(new GetOpportunitiesQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OpportunityDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OpportunityDto>> GetById(Guid id)
    {
        var result = await _sender.Send(new GetOpportunityByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create(CreateOpportunityRequest request)
    {
        var id = await _sender.Send(new CreateOpportunityCommand
        {
            Customer = request.Customer,
            Capacity = request.Capacity,
            Stage = request.Stage,
            Location = request.Location,
            NextAction = request.NextAction,
            Owner = request.Owner,
            Value = request.Value,
            Notes = request.Notes
        });

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, UpdateOpportunityRequest request)
    {
        await _sender.Send(new UpdateOpportunityCommand
        {
            Id = id,
            Code = request.Code,
            Customer = request.Customer,
            Capacity = request.Capacity,
            Stage = request.Stage,
            Location = request.Location,
            NextAction = request.NextAction,
            Owner = request.Owner,
            Value = request.Value
        });

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteOpportunityCommand { Id = id });
        return NoContent();
    }

    [HttpPost("{id:guid}/attachments")]
    [RequestSizeLimit(60_000_000)]
    [ProducesResponseType(typeof(IReadOnlyList<OpportunityAttachmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OpportunityAttachmentDto>>> UploadAttachments(Guid id, [FromForm] IFormFileCollection files)
    {
        var attachmentFiles = new List<AttachmentFile>();
        foreach (var file in files)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            attachmentFiles.Add(new AttachmentFile
            {
                FileName = file.FileName,
                ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                Content = stream.ToArray()
            });
        }

        var result = await _sender.Send(new UploadOpportunityAttachmentsCommand
        {
            OpportunityId = id,
            Files = attachmentFiles
        });

        return Ok(result);
    }

    [HttpGet("{id:guid}/attachments/{attachmentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DownloadAttachment(Guid id, Guid attachmentId)
    {
        var result = await _sender.Send(new GetOpportunityAttachmentQuery { OpportunityId = id, AttachmentId = attachmentId });
        return File(result.Content, result.ContentType, result.FileName);
    }
}

public record CreateOpportunityRequest(string Customer, string Capacity, OpportunityStage Stage, string Location, string NextAction, string Owner, decimal Value, string? Notes);

public record UpdateOpportunityRequest(string Code, string Customer, string Capacity, OpportunityStage Stage, string Location, string NextAction, string Owner, decimal Value);
