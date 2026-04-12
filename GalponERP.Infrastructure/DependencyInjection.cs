using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Infrastructure.Authentication;
using GalponERP.Infrastructure.Notifications;
using GalponERP.Infrastructure.Persistence;
using GalponERP.Infrastructure.Persistence.Repositories;
using GalponERP.Infrastructure.Reporting;
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
        services.AddScoped<ICompraInventarioRepository, CompraInventarioRepository>();
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IProveedorRepository, ProveedorRepository>();
        services.AddScoped<IVentaRepository, VentaRepository>();
        services.AddScoped<IGastoOperativoRepository, GastoOperativoRepository>();
        services.AddScoped<ICalendarioSanitarioRepository, CalendarioSanitarioRepository>();
        services.AddScoped<IPlantillaSanitariaRepository, PlantillaSanitariaRepository>();
        services.AddScoped<IGalponRepository, GalponRepository>();
        services.AddScoped<IMortalidadRepository, MortalidadRepository>();
        services.AddScoped<IPesajeLoteRepository, PesajeLoteRepository>();
        services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
        services.AddScoped<ICategoriaProductoRepository, CategoriaProductoRepository>();
        services.AddScoped<IUnidadMedidaRepository, UnidadMedidaRepository>();
        
        services.AddScoped<IGalponDbContext>(sp => sp.GetRequiredService<GalponDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IAuthenticationService, FirebaseAuthService>();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        services.AddScoped<INotificationService, FirebaseNotificationService>();
        services.AddScoped<IPdfService, PdfService>();
        services.AddHttpContextAccessor();

        return services;
    }
}
