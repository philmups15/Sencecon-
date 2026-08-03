using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.Opportunities.Commands.AddOpportunityNote;
using Sencecon.Application.Opportunities.Commands.ChangeOpportunityStage;
using Sencecon.Application.Opportunities.Commands.ConvertOpportunityToProject;
using Sencecon.Application.Opportunities.Commands.CreateOpportunity;
using Sencecon.Application.Opportunities.Commands.DeleteOpportunity;
using Sencecon.Application.Opportunities.Commands.UpdateOpportunity;
using Sencecon.Application.Opportunities.Commands.UpdateOpportunityStageData;
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
    [Authorize(Policy = "opportunities-read")]
    [ProducesResponseType(typeof(IReadOnlyList<OpportunityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OpportunityDto>>> GetAll()
    {
        var result = await _sender.Send(new GetOpportunitiesQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "opportunities-read")]
    [ProducesResponseType(typeof(OpportunityDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OpportunityDto>> GetById(Guid id)
    {
        var result = await _sender.Send(new GetOpportunityByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "opportunities-write")]
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
            Value = request.Value
        });

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "opportunities-write")]
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
    [Authorize(Policy = "opportunities-write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteOpportunityCommand { Id = id });
        return NoContent();
    }

    [HttpPut("{id:guid}/stage")]
    [Authorize(Policy = "opportunities-write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangeStage(Guid id, ChangeStageRequest request)
    {
        await _sender.Send(new ChangeOpportunityStageCommand
        {
            OpportunityId = id,
            Stage = request.Stage,
            Note = request.Note
        });

        return NoContent();
    }

    [HttpPut("{id:guid}/stage-data")]
    [Authorize(Policy = "opportunities-write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateStageData(Guid id, UpdateStageDataRequest request)
    {
        await _sender.Send(new UpdateOpportunityStageDataCommand
        {
            OpportunityId = id,
            Fields = request.Fields
        });

        return NoContent();
    }

    [HttpPost("{id:guid}/notes")]
    [Authorize(Policy = "opportunities-write")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> AddNote(Guid id, AddNoteRequest request)
    {
        var noteId = await _sender.Send(new AddOpportunityNoteCommand
        {
            OpportunityId = id,
            Text = request.Text
        });

        return CreatedAtAction(nameof(GetById), new { id }, noteId);
    }

    [HttpPost("{id:guid}/convert")]
    [Authorize(Policy = "opportunities-write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Convert(Guid id)
    {
        await _sender.Send(new ConvertOpportunityToProjectCommand { OpportunityId = id });
        return NoContent();
    }

    [HttpPost("{id:guid}/attachments")]
    [Authorize(Policy = "opportunities-write")]
    [RequestSizeLimit(60_000_000)]
    [ProducesResponseType(typeof(IReadOnlyList<OpportunityAttachmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OpportunityAttachmentDto>>> UploadAttachments(Guid id, [FromForm] IFormFileCollection files, [FromForm] string? title)
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
            Files = attachmentFiles,
            Title = title
        });

        return Ok(result);
    }

    [HttpGet("{id:guid}/attachments/{attachmentId:guid}")]
    [Authorize(Policy = "opportunities-read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DownloadAttachment(Guid id, Guid attachmentId)
    {
        var result = await _sender.Send(new GetOpportunityAttachmentQuery { OpportunityId = id, AttachmentId = attachmentId });
        return File(result.Content, result.ContentType, result.FileName);
    }
}

public record CreateOpportunityRequest(string Customer, string Capacity, OpportunityStage Stage, string Location, string NextAction, string Owner, decimal Value);

public record UpdateOpportunityRequest(string Code, string Customer, string Capacity, OpportunityStage Stage, string Location, string NextAction, string Owner, decimal Value);

public record ChangeStageRequest(OpportunityStage Stage, string? Note);

public record UpdateStageDataRequest(Dictionary<string, string> Fields);

public record AddNoteRequest(string Text);
