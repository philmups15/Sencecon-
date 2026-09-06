using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.Surveys.Commands.CreateSurvey;
using Sencecon.Application.Surveys.Commands.DeleteSurvey;
using Sencecon.Application.Surveys.Commands.SurveyChildren;
using Sencecon.Application.Surveys.Commands.UpdateSurvey;
using Sencecon.Application.Surveys.Commands.UploadSurveyPhotos;
using Sencecon.Application.Surveys.Queries.GetSurveyById;
using Sencecon.Application.Surveys.Queries.GetSurveys;
using Sencecon.Domain.Enums;

namespace Sencecon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SurveysController : ControllerBase
{
    private readonly ISender _sender;

    public SurveysController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize(Policy = "surveys-read")]
    [ProducesResponseType(typeof(IReadOnlyList<SurveyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SurveyDto>>> GetAll()
    {
        var result = await _sender.Send(new GetSurveysQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "surveys-read")]
    [ProducesResponseType(typeof(SurveyDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SurveyDto>> GetById(Guid id)
    {
        var result = await _sender.Send(new GetSurveyByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "surveys-write")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create(CreateSurveyRequest request)
    {
        var id = await _sender.Send(new CreateSurveyCommand
        {
            PlantName = request.PlantName,
            Status = request.Status,
            Progress = request.Progress,
            Surveyor = request.Surveyor,
            Date = request.Date,
            ProjectId = request.ProjectId,
            PlantId = request.PlantId
        });

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "surveys-write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, UpdateSurveyRequest request)
    {
        await _sender.Send(new UpdateSurveyCommand
        {
            Id = id,
            Code = request.Code,
            PlantName = request.PlantName,
            Status = request.Status,
            Progress = request.Progress,
            Surveyor = request.Surveyor,
            Date = request.Date,
            ProjectId = request.ProjectId,
            PlantId = request.PlantId
        });

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "surveys-write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteSurveyCommand { Id = id });
        return NoContent();
    }

    // ---- Measurements ----
    [HttpPost("{id:guid}/measurements")]
    [Authorize(Policy = "surveys-write")]
    public async Task<ActionResult<Guid>> AddMeasurement(Guid id, MeasurementRequest request)
        => await _sender.Send(new AddSurveyMeasurementCommand { SurveyId = id, Field = request.Field, Value = request.Value });

    [HttpPut("{id:guid}/measurements/{itemId:guid}")]
    [Authorize(Policy = "surveys-write")]
    public async Task<IActionResult> UpdateMeasurement(Guid id, Guid itemId, MeasurementRequest request)
    {
        await _sender.Send(new UpdateSurveyMeasurementCommand { Id = itemId, Field = request.Field, Value = request.Value });
        return NoContent();
    }

    [HttpDelete("{id:guid}/measurements/{itemId:guid}")]
    [Authorize(Policy = "surveys-write")]
    public async Task<IActionResult> DeleteMeasurement(Guid id, Guid itemId)
    {
        await _sender.Send(new DeleteSurveyMeasurementCommand { Id = itemId });
        return NoContent();
    }

    // ---- Obstructions ----
    [HttpPost("{id:guid}/obstructions")]
    [Authorize(Policy = "surveys-write")]
    public async Task<ActionResult<Guid>> AddObstruction(Guid id, ObstructionRequest request)
        => await _sender.Send(new AddSurveyObstructionCommand { SurveyId = id, Item = request.Item, Impact = request.Impact });

    [HttpPut("{id:guid}/obstructions/{itemId:guid}")]
    [Authorize(Policy = "surveys-write")]
    public async Task<IActionResult> UpdateObstruction(Guid id, Guid itemId, ObstructionRequest request)
    {
        await _sender.Send(new UpdateSurveyObstructionCommand { Id = itemId, Item = request.Item, Impact = request.Impact });
        return NoContent();
    }

    [HttpDelete("{id:guid}/obstructions/{itemId:guid}")]
    [Authorize(Policy = "surveys-write")]
    public async Task<IActionResult> DeleteObstruction(Guid id, Guid itemId)
    {
        await _sender.Send(new DeleteSurveyObstructionCommand { Id = itemId });
        return NoContent();
    }

    // ---- Photos ----
    [HttpPost("{id:guid}/photos")]
    [Authorize(Policy = "surveys-write")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(120_000_000)]
    public async Task<ActionResult<IReadOnlyList<SurveyPhotoDto>>> UploadPhotos(Guid id, [FromForm] IFormFileCollection files, [FromForm] string? title, [FromForm] string? gps)
    {
        var photoFiles = new List<SurveyPhotoFile>();
        foreach (var file in files)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            photoFiles.Add(new SurveyPhotoFile
            {
                FileName = file.FileName,
                ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                Content = stream.ToArray()
            });
        }
        return Ok(await _sender.Send(new UploadSurveyPhotosCommand { SurveyId = id, Files = photoFiles, Title = title, Gps = gps }));
    }

    [HttpGet("{id:guid}/photos/{photoId:guid}")]
    [Authorize(Policy = "surveys-read")]
    public async Task<IActionResult> DownloadPhoto(Guid id, Guid photoId)
    {
        var result = await _sender.Send(new GetSurveyPhotoQuery { SurveyId = id, PhotoId = photoId });
        return File(result.Content, result.ContentType, result.FileName);
    }

    [HttpDelete("{id:guid}/photos/{photoId:guid}")]
    [Authorize(Policy = "surveys-write")]
    public async Task<IActionResult> DeletePhoto(Guid id, Guid photoId)
    {
        await _sender.Send(new DeleteSurveyPhotoCommand { SurveyId = id, PhotoId = photoId });
        return NoContent();
    }
}

public record MeasurementRequest(string Field, string Value);
public record ObstructionRequest(string Item, string Impact);

public record CreateSurveyRequest(string PlantName, SurveyStatus Status, int Progress, string Surveyor, DateTimeOffset Date, Guid? ProjectId, Guid? PlantId);

public record UpdateSurveyRequest(string Code, string PlantName, SurveyStatus Status, int Progress, string Surveyor, DateTimeOffset Date, Guid? ProjectId, Guid? PlantId);
