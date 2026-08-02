using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Infrastructure.Email;
using Sencecon.Infrastructure.Identity;
using Sencecon.Infrastructure.Persistence;

namespace Sencecon.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<ResendSettings>(configuration.GetSection(ResendSettings.SectionName));
        services.Configure<FrontendSettings>(configuration.GetSection(FrontendSettings.SectionName));

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddHttpClient<IEmailService, ResendEmailService>();
        services.AddScoped<ISecretProtector, DataProtectionSecretProtector>();

        // Persist the key ring to the DB rather than the container filesystem —
        // Railway's containers are ephemeral across deploys, so the default
        // file-system key store would regenerate on every deploy and silently
        // break decryption of anything encrypted with the previous keys.
        services.AddDataProtection()
            .PersistKeysToDbContext<ApplicationDbContext>()
            .SetApplicationName("Sencecon");

        return services;
    }
}
