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
        });

        services.AddValidatorsFromAssembly(assembly);
        services.AddHttpClient();

        services.AddScoped<GalponERP.Domain.Services.CalculadoraCostosLote>();
        services.AddScoped<GalponERP.Domain.Services.SimuladorProyeccionLote>();

        return services;
    }
}
