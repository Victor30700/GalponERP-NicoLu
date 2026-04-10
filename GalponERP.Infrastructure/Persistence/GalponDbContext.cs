using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence;

public class GalponDbContext : DbContext
{
    public GalponDbContext(DbContextOptions<GalponDbContext> options) : base(options)
    {
    }

    public DbSet<Galpon> Galpones { get; set; }
    public DbSet<Lote> Lotes { get; set; }
    public DbSet<MortalidadDiaria> Mortalidades { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<MovimientoInventario> MovimientosInventario { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<GastoOperativo> GastosOperativos { get; set; }
    public DbSet<CalendarioSanitario> CalendarioSanitario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GalponDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
