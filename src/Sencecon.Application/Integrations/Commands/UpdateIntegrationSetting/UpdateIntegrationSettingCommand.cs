using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Integrations.Queries.GetIntegrationSettings;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Integrations.Commands.UpdateIntegrationSetting;

public record UpdateIntegrationSettingCommand : IRequest<IntegrationSettingDto>
{
    public required string Key { get; init; }
    public string? ProviderEndpoint { get; init; }

    // Null/omitted: leave the stored key untouched. Empty string is treated the
    // same as ClearApiKey — see the validator/handler below.
    public string? ApiKey { get; init; }
    public bool ClearApiKey { get; init; }
    public string? Notes { get; init; }
}

public class UpdateIntegrationSettingCommandValidator : AbstractValidator<UpdateIntegrationSettingCommand>
{
    public UpdateIntegrationSettingCommandValidator()
    {
        RuleFor(v => v.Key)
            .NotEmpty()
            .Must(IntegrationCatalogue.IsKnown)
            .WithMessage("Unknown integration key.");

        RuleFor(v => v.ProviderEndpoint).MaximumLength(500);
        RuleFor(v => v.ApiKey).MaximumLength(2000);
        RuleFor(v => v.Notes).MaximumLength(1000);
    }
}

public class UpdateIntegrationSettingCommandHandler : IRequestHandler<UpdateIntegrationSettingCommand, IntegrationSettingDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ISecretProtector _secretProtector;

    public UpdateIntegrationSettingCommandHandler(IApplicationDbContext context, ISecretProtector secretProtector)
    {
        _context = context;
        _secretProtector = secretProtector;
    }

    public async Task<IntegrationSettingDto> Handle(UpdateIntegrationSettingCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.IntegrationSettings
            .FirstOrDefaultAsync(i => i.Key == request.Key, cancellationToken);

        if (entity is null)
        {
            entity = new IntegrationSetting { Key = request.Key, Created = DateTimeOffset.UtcNow };
            _context.IntegrationSettings.Add(entity);
        }

        entity.ProviderEndpoint = request.ProviderEndpoint;
        entity.Notes = request.Notes;

        if (request.ClearApiKey || request.ApiKey is "")
        {
            entity.ApiKeyCipher = null;
        }
        else if (!string.IsNullOrEmpty(request.ApiKey))
        {
            entity.ApiKeyCipher = _secretProtector.Protect(request.ApiKey);
        }
        // else: ApiKey was null (not provided) — leave the existing cipher as-is.

        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var isConfigured = !string.IsNullOrEmpty(entity.ProviderEndpoint) || !string.IsNullOrEmpty(entity.ApiKeyCipher);

        return new IntegrationSettingDto
        {
            Key = entity.Key,
            Name = IntegrationCatalogue.NameFor(entity.Key) ?? entity.Key,
            Status = isConfigured ? "Connected" : "Not configured",
            ProviderEndpoint = entity.ProviderEndpoint,
            HasApiKey = !string.IsNullOrEmpty(entity.ApiKeyCipher),
            Notes = entity.Notes,
            LastModified = entity.LastModified
        };
    }
}
