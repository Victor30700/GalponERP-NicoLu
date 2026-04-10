using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Infrastructure.Authentication;
using GalponERP.Infrastructure.Notifications;
using GalponERP.Infrastructure.Persistence;
using GalponERP.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GalponERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<GalponDbContext>(options =>
            options.UseNpgsql(connectionString,
                b => b.MigrationsAssembly(typeof(GalponDbContext).Assembly.FullName)));

        services.AddScoped<ILoteRepository, LoteRepository>();
        services.AddScoped<IInventarioRepository, InventarioRepository>();
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IVentaRepository, VentaRepository>();
        services.AddScoped<IGastoOperativoRepository, GastoOperativoRepository>();
        services.AddScoped<ICalendarioSanitarioRepository, CalendarioSanitarioRepository>();
        services.AddScoped<IGalponRepository, GalponRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IAuthenticationService, FirebaseAuthService>();
        services.AddScoped<INotificationService, FirebaseNotificationService>();
        services.AddHttpContextAccessor();

        return services;
    }
}
