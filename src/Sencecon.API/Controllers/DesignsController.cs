using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.Designs.Commands.CreateDesign;
using Sencecon.Application.Designs.Commands.DeleteDesign;
using Sencecon.Application.Designs.Commands.UpdateDesign;
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
            Code = request.Code,
            ProjectName = request.ProjectName,
            Status = request.Status,
            Revision = request.Revision,
            SurveyId = request.SurveyId
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
            Code = request.Code,
            ProjectName = request.ProjectName,
            Status = request.Status,
            Revision = request.Revision,
            SurveyId = request.SurveyId
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
}

public record CreateDesignRequest(string Code, string ProjectName, DesignStatus Status, string Revision, Guid? SurveyId);

public record UpdateDesignRequest(string Code, string ProjectName, DesignStatus Status, string Revision, Guid? SurveyId);
