using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.API.Authorization;
using Sencecon.Application.AuditLog.Queries.GetAuditLog;

namespace Sencecon.API.Controllers;

[ApiController]
[Route("api/auditlog")]
[Authorize(Roles = Roles.Admin)]
public class AuditLogController : ControllerBase
{
    private readonly ISender _sender;

    public AuditLogController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AuditLogEntryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AuditLogEntryDto>>> GetAll()
    {
        var result = await _sender.Send(new GetAuditLogQuery());
        return Ok(result);
    }
}
