using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.API.Authorization;
using Sencecon.Application.Integrations.Commands.UpdateIntegrationSetting;
using Sencecon.Application.Integrations.Queries.GetIntegrationSettings;

namespace Sencecon.API.Controllers;

// Admin-only end to end: only Admins reach the Integrations tab in the frontend
// (it's nested inside the Admin screen, which is already role-gated), so both
// read and write are gated the same way here rather than via ModuleAccess.
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class IntegrationsController : ControllerBase
{
    private readonly ISender _sender;

    public IntegrationsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<IntegrationSettingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<IntegrationSettingDto>>> GetAll()
    {
        var result = await _sender.Send(new GetIntegrationSettingsQuery());
        return Ok(result);
    }

    [HttpPut("{key}")]
    [ProducesResponseType(typeof(IntegrationSettingDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<IntegrationSettingDto>> Update(string key, UpdateIntegrationSettingRequest request)
    {
        var result = await _sender.Send(new UpdateIntegrationSettingCommand
        {
            Key = key,
            ProviderEndpoint = request.ProviderEndpoint,
            ApiKey = request.ApiKey,
            ClearApiKey = request.ClearApiKey,
            Notes = request.Notes
        });

        return Ok(result);
    }
}

public record UpdateIntegrationSettingRequest(string? ProviderEndpoint, string? ApiKey, bool ClearApiKey, string? Notes);
