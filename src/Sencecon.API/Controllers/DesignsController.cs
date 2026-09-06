using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.Designs.Commands.CreateDesign;
using Sencecon.Application.Designs.Commands.DeleteDesign;
using Sencecon.Application.Designs.Commands.DesignRevisionSpecs;
using Sencecon.Application.Designs.Commands.UpdateDesign;
using Sencecon.Application.Designs.Commands.UploadDesignAttachments;
using Sencecon.Application.Designs.Queries.GetDesignAttachment;
using Sencecon.Application.Designs.Queries.GetDesignAttachments;
using Sencecon.Application.Designs.Queries.GetDesignById;
using Sencecon.Application.Designs.Queries.GetDesigns;
using Sencecon.Domain.Enums;

namespace Sencecon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DesignsController : ControllerBase
{
    private readonly ISender _sender;

    public DesignsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize(Policy = "designs-read")]
    [ProducesResponseType(typeof(IReadOnlyList<DesignDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DesignDto>>> GetAll()
    {
        var result = await _sender.Send(new GetDesignsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "designs-read")]
    [ProducesResponseType(typeof(DesignDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DesignDto>> GetById(Guid id)
    {
        var result = await _sender.Send(new GetDesignByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "designs-write")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create(CreateDesignRequest request)
    {
        var id = await _sender.Send(new CreateDesignCommand
        {
            ProjectName = request.ProjectName,
            Status = request.Status,
            Revision = request.Revision,
            SurveyId = request.SurveyId,
            ProjectId = request.ProjectId
        });

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "designs-write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, UpdateDesignRequest request)
    {
        await _sender.Send(new UpdateDesignCommand
        {
            Id = id,
            ProjectName = request.ProjectName,
            Status = request.Status,
            Revision = request.Revision,
            SurveyId = request.SurveyId,
            ProjectId = request.ProjectId
        });

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "designs-write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteDesignCommand { Id = id });
        return NoContent();
    }

    [HttpGet("{id:guid}/attachments")]
    [Authorize(Policy = "designs-read")]
    public async Task<ActionResult<IReadOnlyList<DesignAttachmentDto>>> GetAttachments(Guid id)
        => Ok(await _sender.Send(new GetDesignAttachmentsQuery { DesignId = id }));

    [HttpPost("{id:guid}/attachments")]
    [Authorize(Policy = "designs-write")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(60_000_000)]
    public async Task<ActionResult<IReadOnlyList<DesignAttachmentDto>>> UploadAttachments(Guid id, [FromForm] IFormFileCollection files, [FromForm] string? title)
    {
        var attachmentFiles = new List<DesignAttachmentFile>();
        foreach (var file in files)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            attachmentFiles.Add(new DesignAttachmentFile
            {
                FileName = file.FileName,
                ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                Content = stream.ToArray()
            });
        }
        return Ok(await _sender.Send(new UploadDesignAttachmentsCommand { DesignId = id, Files = attachmentFiles, Title = title }));
    }

    [HttpGet("{id:guid}/attachments/{attachmentId:guid}")]
    [Authorize(Policy = "designs-read")]
    public async Task<IActionResult> DownloadAttachment(Guid id, Guid attachmentId)
    {
        var result = await _sender.Send(new GetDesignAttachmentQuery { DesignId = id, AttachmentId = attachmentId });
        return File(result.Content, result.ContentType, result.FileName);
    }

    [HttpPost("{id:guid}/revisions")]
    [Authorize(Policy = "designs-write")]
    public async Task<ActionResult<Guid>> AddRevision(Guid id, AddDesignRevisionRequest request)
        => await _sender.Send(new AddDesignRevisionCommand { DesignId = id, Revision = request.Revision, Note = request.Note });

    [HttpPut("{id:guid}/specs/{tab}")]
    [Authorize(Policy = "designs-write")]
    public async Task<IActionResult> UpdateSpecs(Guid id, string tab, UpdateDesignSpecsRequest request)
    {
        await _sender.Send(new UpdateDesignSpecsCommand { DesignId = id, Tab = tab, Fields = request.Fields });
        return NoContent();
    }
}

public record AddDesignRevisionRequest(string Revision, string? Note);
public record UpdateDesignSpecsRequest(Dictionary<string, string> Fields);

public record CreateDesignRequest(string ProjectName, DesignStatus Status, string Revision, Guid? SurveyId, Guid? ProjectId);

public record UpdateDesignRequest(string ProjectName, DesignStatus Status, string Revision, Guid? SurveyId, Guid? ProjectId);
