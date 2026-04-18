using GalponERP.Domain.Entities;
using GalponERP.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class PagoVentaConfiguration : IEntityTypeConfiguration<PagoVenta>
{
    public void Configure(EntityTypeBuilder<PagoVenta> builder)
    {
        builder.ToTable("PagosVentas");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Monto)
            .HasConversion(
                m => m.Monto,
                v => new Moneda(v))
            .HasColumnName("Monto")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.FechaPago)
            .IsRequired();

        builder.Property(p => p.MetodoPago)
            .IsRequired();

        builder.Property(p => p.UsuarioId)
            .IsRequired();

        builder.HasIndex(p => p.VentaId);
        builder.HasIndex(p => p.FechaPago);

        builder.HasQueryFilter(p => p.IsActive);
    }
}
