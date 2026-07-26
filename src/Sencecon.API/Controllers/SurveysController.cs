using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.Surveys.Commands.CreateSurvey;
using Sencecon.Application.Surveys.Commands.DeleteSurvey;
using Sencecon.Application.Surveys.Commands.UpdateSurvey;
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
    [ProducesResponseType(typeof(IReadOnlyList<SurveyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SurveyDto>>> GetAll()
    {
        var result = await _sender.Send(new GetSurveysQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SurveyDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SurveyDto>> GetById(Guid id)
    {
        var result = await _sender.Send(new GetSurveyByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create(CreateSurveyRequest request)
    {
        var id = await _sender.Send(new CreateSurveyCommand
        {
            Code = request.Code,
            PlantName = request.PlantName,
            Status = request.Status,
            Progress = request.Progress,
            Surveyor = request.Surveyor,
            Date = request.Date
        });

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
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
            Date = request.Date
        });

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteSurveyCommand { Id = id });
        return NoContent();
    }
}

public record CreateSurveyRequest(string Code, string PlantName, SurveyStatus Status, int Progress, string Surveyor, DateTimeOffset Date);

public record UpdateSurveyRequest(string Code, string PlantName, SurveyStatus Status, int Progress, string Surveyor, DateTimeOffset Date);
