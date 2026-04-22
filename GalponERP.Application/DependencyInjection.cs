using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace GalponERP.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(config => 
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddOpenBehavior(typeof(GalponERP.Application.Behaviors.ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(GalponERP.Application.Behaviors.AuditoriaBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);
        services.AddHttpClient();

        services.AddScoped<GalponERP.Domain.Services.CalculadoraCostosLote>();
        services.AddScoped<GalponERP.Domain.Services.SimuladorProyeccionLote>();
        services.AddScoped<GalponERP.Domain.Interfaces.Services.ISanidadService, GalponERP.Domain.Services.SanidadService>();

        return services;
    }
}
