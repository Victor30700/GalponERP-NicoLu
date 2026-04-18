using GalponERP.Domain.Entities;
using GalponERP.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class VentaConfiguration : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.ToTable("Ventas");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Fecha)
            .IsRequired();

        builder.Property(v => v.CantidadPollos)
            .IsRequired();

        builder.Property(v => v.PesoTotalVendido)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(v => v.PrecioPorKilo)
            .HasConversion(
                m => m.Monto,
                v => new Moneda(v))
            .HasColumnName("PrecioPorKilo")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(v => v.Total)
            .HasConversion(
                m => m.Monto,
                v => new Moneda(v))
            .HasColumnName("TotalVenta")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne<Lote>()
            .WithMany()
            .HasForeignKey(v => v.LoteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(v => v.EstadoPago)
            .IsRequired()
            .HasDefaultValue(EstadoPago.Pendiente)
            .HasSentinel((EstadoPago)0);

        builder.HasMany(v => v.Pagos)
            .WithOne()
            .HasForeignKey(p => p.VentaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Venta.Pagos))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(v => v.LoteId);
        builder.HasIndex(v => v.ClienteId);
        builder.HasIndex(v => v.Fecha);
    }
}
