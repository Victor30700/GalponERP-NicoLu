using GalponERP.Domain.Entities;
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

        builder.ComplexProperty(v => v.PrecioPorKilo, b =>
        {
            b.Property(p => p.Monto)
                .HasColumnName("PrecioPorKilo")
                .HasPrecision(18, 2);
        });

        builder.ComplexProperty(v => v.Total, b =>
        {
            b.Property(p => p.Monto)
                .HasColumnName("TotalVenta")
                .HasPrecision(18, 2);
        });

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
            .HasDefaultValue(EstadoPago.Pagado);

        builder.HasIndex(v => v.LoteId);
        builder.HasIndex(v => v.ClienteId);
        builder.HasIndex(v => v.Fecha);

        builder.HasQueryFilter(v => v.IsActive);
    }
}
