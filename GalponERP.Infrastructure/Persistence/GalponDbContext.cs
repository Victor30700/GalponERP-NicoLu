using GalponERP.Domain.Entities;
using GalponERP.Domain.Primitives;
using GalponERP.Domain.ValueObjects;
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
    public DbSet<InventarioLote> InventarioLotes { get; set; }
    public DbSet<MortalidadDiaria> Mortalidades { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<CategoriaProducto> CategoriasProductos { get; set; }
    public DbSet<UnidadMedida> UnidadesMedida { get; set; }
    public DbSet<MovimientoInventario> MovimientosInventario { get; set; }
    public DbSet<Formula> Formulas { get; set; }
    public DbSet<FormulaDetalle> FormulaDetalles { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<PagoVenta> PagosVentas { get; set; }
    public DbSet<PagoCompra> PagosCompras { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Proveedor> Proveedores { get; set; }
    public DbSet<CompraInventario> ComprasInventario { get; set; }
    public DbSet<OrdenCompra> OrdenesCompra { get; set; }
    public DbSet<OrdenCompraItem> OrdenesCompraItems { get; set; }
    public DbSet<GastoOperativo> GastosOperativos { get; set; }
    public DbSet<PlantillaSanitaria> PlantillasSanitarias { get; set; }
    public DbSet<CalendarioSanitario> CalendarioSanitario { get; set; }
    public DbSet<RegistroBienestar> RegistroBienestar { get; set; }
    public DbSet<PesajeLote> PesajesLote { get; set; }
    public DbSet<ConfiguracionSistema> Configuracion { get; set; }
    public DbSet<AuditoriaLog> AuditoriaLogs { get; set; }
    public DbSet<Conversacion> Conversaciones { get; set; }
    public DbSet<MensajeChat> MensajesChat { get; set; }
    public DbSet<IntencionPendiente> IntencionesPendientes { get; set; }

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
            else if (entry.State == EntityState.Deleted)
            {
                // Soft Delete: Convertir Delete en Update de IsActive
                entry.State = EntityState.Modified;
                entry.Entity.Desactivar();
                entry.Entity.SetAuditoriaModificacion(now, usuarioId);
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GalponDbContext).Assembly);

        // Global Query Filter for Soft Delete (IsActive)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(Entity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(ConvertFilterExpression<Entity>(e => e.IsActive, entityType.ClrType));
                
                // Configuración de Concurrencia Optimista usando xmin de PostgreSQL
                modelBuilder.Entity(entityType.ClrType)
                    .Property<uint>("Version")
                    .HasColumnName("xmin")
                    .HasColumnType("xid")
                    .ValueGeneratedOnAddOrUpdate()
                    .IsConcurrencyToken();
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    private static System.Linq.Expressions.LambdaExpression ConvertFilterExpression<TInterface>(
        System.Linq.Expressions.Expression<Func<TInterface, bool>> filterExpression,
        Type entityType)
    {
        var newParam = System.Linq.Expressions.Expression.Parameter(entityType);
        var newBody = ReplacingExpressionVisitor.Replace(filterExpression.Parameters.Single(), newParam, filterExpression.Body);
        return System.Linq.Expressions.Expression.Lambda(newBody, newParam);
    }
}

// Required internal class for replacing the expression parameter type
internal class ReplacingExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
{
    private readonly System.Linq.Expressions.Expression _oldValue;
    private readonly System.Linq.Expressions.Expression _newValue;

    public ReplacingExpressionVisitor(System.Linq.Expressions.Expression oldValue, System.Linq.Expressions.Expression newValue)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    public override System.Linq.Expressions.Expression? Visit(System.Linq.Expressions.Expression? node)
    {
        if (node == _oldValue)
            return _newValue;
        return base.Visit(node);
    }

    public static System.Linq.Expressions.Expression Replace(System.Linq.Expressions.Expression oldValue, System.Linq.Expressions.Expression newValue, System.Linq.Expressions.Expression expression)
    {
        return new ReplacingExpressionVisitor(oldValue, newValue).Visit(expression)!;
    }
}

