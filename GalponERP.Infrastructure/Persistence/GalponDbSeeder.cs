using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GalponERP.Infrastructure.Persistence;

public static class GalponDbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GalponDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GalponDbContext>>();

        try
        {
            if (context.Database.IsNpgsql())
            {
                await context.Database.MigrateAsync();
            }

            await SeedUnidadesMedidaAsync(context);
            await SeedCategoriasAsync(context);

            await context.SaveChangesAsync();
            logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    private static async Task SeedUnidadesMedidaAsync(GalponDbContext context)
    {
        if (!await context.UnidadesMedida.AnyAsync())
        {
            context.UnidadesMedida.AddRange(
                new UnidadMedida(Guid.NewGuid(), "Kilogramo", "Kg"),
                new UnidadMedida(Guid.NewGuid(), "Unidad individual", "Und"),
                new UnidadMedida(Guid.NewGuid(), "Dosis (Medicina)", "Dos"),
                new UnidadMedida(Guid.NewGuid(), "Bulto de 50 Kg", "Bulto")
            );
        }
    }

    private static async Task SeedCategoriasAsync(GalponDbContext context)
    {
        if (!await context.CategoriasProductos.AnyAsync())
        {
            context.CategoriasProductos.AddRange(
                new CategoriaProducto(Guid.NewGuid(), "Alimento", "Alimento balanceado para aves"),
                new CategoriaProducto(Guid.NewGuid(), "Medicina", "Medicamentos en general"),
                new CategoriaProducto(Guid.NewGuid(), "Vacuna", "Vacunas preventivas"),
                new CategoriaProducto(Guid.NewGuid(), "Vitamina", "Vitaminas y suplementos")
            );
        }
    }
}
