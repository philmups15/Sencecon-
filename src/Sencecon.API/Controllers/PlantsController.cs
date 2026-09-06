using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.Plants.Commands.CreatePlant;
using Sencecon.Application.Plants.Commands.DeletePlant;
using Sencecon.Application.Plants.Commands.RecordCommissioningTestResult;
using Sencecon.Application.Plants.Commands.UpdatePlant;
using Sencecon.Application.Plants.Commands.UploadPlantAttachments;
using Sencecon.Application.Plants.Handover;
using Sencecon.Application.Plants.Queries.GetCommissioningChecklist;
using Sencecon.Application.Plants.Queries.GetCommissioningTestResults;
using Sencecon.Application.Plants.Queries.GetPlantAttachment;
using Sencecon.Application.Plants.Queries.GetPlantAttachments;
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
            Name = request.Name,
            Stage = request.Stage,
            Type = request.Type,
            Capacity = request.Capacity,
            Equipment = request.Equipment,
            PerformanceRatio = request.PerformanceRatio,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Health = request.Health,
            ProjectId = request.ProjectId
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
            Name = request.Name,
            Stage = request.Stage,
            Type = request.Type,
            Capacity = request.Capacity,
            Equipment = request.Equipment,
            PerformanceRatio = request.PerformanceRatio,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Health = request.Health,
            ProjectId = request.ProjectId
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

    [HttpGet("{id:guid}/attachments")]
    [Authorize(Policy = "plants-read")]
    [ProducesResponseType(typeof(IReadOnlyList<PlantAttachmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PlantAttachmentDto>>> GetAttachments(Guid id)
    {
        var result = await _sender.Send(new GetPlantAttachmentsQuery { PlantId = id });
        return Ok(result);
    }

    [HttpPost("{id:guid}/attachments")]
    [Authorize(Policy = "plants-write")]
    [RequestSizeLimit(60_000_000)]
    [ProducesResponseType(typeof(IReadOnlyList<PlantAttachmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PlantAttachmentDto>>> UploadAttachments(Guid id, [FromForm] IFormFileCollection files, [FromForm] string? title)
    {
        var attachmentFiles = new List<PlantAttachmentFile>();
        foreach (var file in files)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            attachmentFiles.Add(new PlantAttachmentFile
            {
                FileName = file.FileName,
                ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                Content = stream.ToArray()
            });
        }

        var result = await _sender.Send(new UploadPlantAttachmentsCommand
        {
            PlantId = id,
            Files = attachmentFiles,
            Title = title
        });

        return Ok(result);
    }

    [HttpGet("{id:guid}/attachments/{attachmentId:guid}")]
    [Authorize(Policy = "plants-read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DownloadAttachment(Guid id, Guid attachmentId)
    {
        var result = await _sender.Send(new GetPlantAttachmentQuery { PlantId = id, AttachmentId = attachmentId });
        return File(result.Content, result.ContentType, result.FileName);
    }

    [HttpGet("{id:guid}/commissioning-tests")]
    [Authorize(Policy = "plants-read")]
    [ProducesResponseType(typeof(IReadOnlyList<CommissioningTestResultDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CommissioningTestResultDto>>> GetCommissioningTests(Guid id)
    {
        var result = await _sender.Send(new GetCommissioningTestResultsQuery { PlantId = id });
        return Ok(result);
    }

    [HttpGet("{id:guid}/commissioning-checklist")]
    [Authorize(Policy = "plants-read")]
    [ProducesResponseType(typeof(IReadOnlyList<CommissioningChecklistRowDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CommissioningChecklistRowDto>>> GetCommissioningChecklist(Guid id)
    {
        var result = await _sender.Send(new GetCommissioningChecklistQuery { PlantId = id });
        return Ok(result);
    }

    [HttpPut("{id:guid}/commissioning-tests")]
    [Authorize(Policy = "plants-write")]
    [ProducesResponseType(typeof(CommissioningTestResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CommissioningTestResultDto>> RecordCommissioningTest(Guid id, RecordCommissioningTestRequest request)
    {
        var result = await _sender.Send(new RecordCommissioningTestResultCommand
        {
            PlantId = id,
            Category = request.Category,
            TestName = request.TestName,
            Result = request.Result,
            Notes = request.Notes
        });

        return Ok(result);
    }

    // ---- Handover ----
    [HttpGet("{id:guid}/handover")]
    [Authorize(Policy = "plants-read")]
    [ProducesResponseType(typeof(HandoverDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<HandoverDto>> GetHandover(Guid id)
        => Ok(await _sender.Send(new GetHandoverQuery { PlantId = id }));

    [HttpPut("{id:guid}/handover")]
    [Authorize(Policy = "plants-write")]
    public async Task<IActionResult> UpsertHandover(Guid id, UpsertHandoverRequest request)
    {
        await _sender.Send(new UpsertHandoverCommand { PlantId = id, AcceptanceDate = request.AcceptanceDate, Notes = request.Notes });
        return NoContent();
    }

    [HttpPost("{id:guid}/handover/sign-off")]
    [Authorize(Policy = "plants-write")]
    public async Task<IActionResult> SignOffHandover(Guid id)
    {
        await _sender.Send(new SignOffHandoverCommand { PlantId = id });
        return NoContent();
    }

    [HttpPost("{id:guid}/handover/certificate")]
    [Authorize(Policy = "plants-write")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(12_000_000)]
    public async Task<IActionResult> UploadHandoverCertificate(Guid id, [FromForm] IFormFile file)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        await _sender.Send(new UploadHandoverCertificateCommand
        {
            PlantId = id,
            FileName = file.FileName,
            ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
            Content = stream.ToArray()
        });
        return NoContent();
    }

    [HttpGet("{id:guid}/handover/certificate")]
    [Authorize(Policy = "plants-read")]
    public async Task<IActionResult> DownloadHandoverCertificate(Guid id)
    {
        var result = await _sender.Send(new GetHandoverCertificateQuery { PlantId = id });
        return File(result.Content, result.ContentType, result.FileName);
    }
}

public record UpsertHandoverRequest(DateTimeOffset? AcceptanceDate, string? Notes);

public record CreatePlantRequest(string Name, LifecycleStage Stage, PlantType Type, string Capacity, string Equipment, double? PerformanceRatio, double? Latitude, double? Longitude, PlantHealth Health, Guid? ProjectId);

public record UpdatePlantRequest(string Name, LifecycleStage Stage, PlantType Type, string Capacity, string Equipment, double? PerformanceRatio, double? Latitude, double? Longitude, PlantHealth Health, Guid? ProjectId);

public record RecordCommissioningTestRequest(CommissioningTestCategory Category, string TestName, CommissioningResultStatus Result, string? Notes);
