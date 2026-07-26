using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Sencecon.Application.Common.Behaviours;

namespace Sencecon.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(AuditLoggingBehaviour<,>));
        });

        return services;
    }
}
