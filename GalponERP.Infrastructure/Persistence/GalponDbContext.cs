using GalponERP.Domain.Entities;
using GalponERP.Domain.Primitives;
using GalponERP.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence;

public class GalponDbContext : DbContext, IGalponDbContext
{
    private readonly ICurrentUserContext _currentUserContext;

    public GalponDbContext(
        DbContextOptions<GalponDbContext> options,
        ICurrentUserContext currentUserContext) : base(options)
    {
        _currentUserContext = currentUserContext;
    }

    public DbSet<Galpon> Galpones { get; set; }
    public DbSet<Lote> Lotes { get; set; }
    public DbSet<MortalidadDiaria> Mortalidades { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<CategoriaProducto> CategoriasProductos { get; set; }
    public DbSet<UnidadMedida> UnidadesMedida { get; set; }
    public DbSet<MovimientoInventario> MovimientosInventario { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<PagoVenta> PagosVentas { get; set; }
    public DbSet<PagoCompra> PagosCompras { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Proveedor> Proveedores { get; set; }
    public DbSet<CompraInventario> ComprasInventario { get; set; }
    public DbSet<GastoOperativo> GastosOperativos { get; set; }
    public DbSet<PlantillaSanitaria> PlantillasSanitarias { get; set; }
    public DbSet<CalendarioSanitario> CalendarioSanitario { get; set; }
    public DbSet<PesajeLote> PesajesLote { get; set; }
    public DbSet<AuditoriaLog> AuditoriaLogs { get; set; }

    public async Task<T?> ObtenerEntidadPorIdAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : Entity
    {
        return await Set<T>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<Entity>();
        var usuarioId = _currentUserContext.UsuarioId ?? Guid.Empty;
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.SetAuditoriaCreacion(now, usuarioId);
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.SetAuditoriaModificacion(now, usuarioId);
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GalponDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
