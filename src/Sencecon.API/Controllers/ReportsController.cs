using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.Reports.Commands.CreateReport;
using Sencecon.Application.Reports.Commands.DeleteReport;
using Sencecon.Application.Reports.Queries.GetReports;

namespace Sencecon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ISender _sender;

    public ReportsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize(Policy = "reports-read")]
    [ProducesResponseType(typeof(IReadOnlyList<ReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ReportDto>>> GetAll()
    {
        var result = await _sender.Send(new GetReportsQuery());
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "reports-write")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create(CreateReportRequest request)
    {
        var id = await _sender.Send(new CreateReportCommand
        {
            Name = request.Name,
            GeneratedBy = request.GeneratedBy
        });

        return CreatedAtAction(nameof(GetAll), new { id }, id);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "reports-write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteReportCommand { Id = id });
        return NoContent();
    }
}

public record CreateReportRequest(string Name, string GeneratedBy);
