using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class PagoVentaConfiguration : IEntityTypeConfiguration<PagoVenta>
{
    public void Configure(EntityTypeBuilder<PagoVenta> builder)
    {
        builder.ToTable("PagosVentas");

        builder.HasKey(p => p.Id);

        builder.ComplexProperty(p => p.Monto, b =>
        {
            b.Property(m => m.Monto)
                .HasColumnName("Monto")
                .HasPrecision(18, 2);
        });

        builder.Property(p => p.FechaPago)
            .IsRequired();

        builder.Property(p => p.MetodoPago)
            .IsRequired();

        builder.Property(p => p.UsuarioId)
            .IsRequired();

        builder.HasOne<Venta>()
            .WithMany(v => v.Pagos)
            .HasForeignKey(p => p.VentaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.VentaId);
        builder.HasIndex(p => p.FechaPago);

        builder.HasQueryFilter(p => p.IsActive);
    }
}
