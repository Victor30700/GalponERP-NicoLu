using GalponERP.Domain.Entities;
using GalponERP.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class MovimientoInventarioConfiguration : IEntityTypeConfiguration<MovimientoInventario>
{
    public void Configure(EntityTypeBuilder<MovimientoInventario> builder)
    {
        builder.ToTable("MovimientosInventario");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Cantidad)
            .IsRequired()
            .HasPrecision(18, 6);

        builder.Property(m => m.PesoUnitarioHistorico)
            .IsRequired()
            .HasPrecision(18, 6)
            .HasDefaultValue(0);

        builder.Property(m => m.Tipo)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.Fecha)
            .IsRequired();

        // Usamos Conversión de Valor en lugar de ComplexProperty para manejar la nulabilidad de Moneda? correctamente
        builder.Property(m => m.CostoTotal)
            .HasConversion(
                moneda => moneda != null ? moneda.Monto : (decimal?)null,
                valor => valor.HasValue ? new Moneda(valor.Value) : null
            )
            .HasColumnName("CostoTotal")
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(m => m.Proveedor)
            .HasMaxLength(200);

        builder.HasOne(m => m.Producto)
            .WithMany()
            .HasForeignKey(m => m.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Lote)
            .WithMany()
            .HasForeignKey(m => m.LoteId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<CompraInventario>()
            .WithMany()
            .HasForeignKey(m => m.CompraId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(m => m.ProductoId);
        builder.HasIndex(m => m.LoteId);
        builder.HasIndex(m => m.CompraId);
        builder.HasIndex(m => m.Fecha);

        builder.HasQueryFilter(m => m.IsActive);
    }
}
