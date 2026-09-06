using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.API.Authorization;
using Sencecon.Application.Commissioning;
using Sencecon.Domain.Enums;

namespace Sencecon.API.Controllers;

[ApiController]
[Route("api/commissioning-templates")]
[Authorize(Roles = Roles.Admin)]
public class CommissioningTemplatesController : ControllerBase
{
    private readonly ISender _sender;

    public CommissioningTemplatesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CommissioningTemplateDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CommissioningTemplateDto>>> GetAll()
        => Ok(await _sender.Send(new GetCommissioningTemplatesQuery()));

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<ActionResult<Guid>> Upsert(UpsertCommissioningTemplateRequest request)
        => await _sender.Send(new UpsertCommissioningTemplateCommand
        {
            Id = request.Id,
            Category = request.Category,
            TestName = request.TestName,
            AppliesToTypes = request.AppliesToTypes ?? Array.Empty<PlantType>(),
            Order = request.Order,
            IsActive = request.IsActive
        });

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteCommissioningTemplateCommand { Id = id });
        return NoContent();
    }
}

public record UpsertCommissioningTemplateRequest(Guid? Id, CommissioningTestCategory Category, string TestName, IReadOnlyList<PlantType>? AppliesToTypes, int Order, bool IsActive);
