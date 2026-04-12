using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class PagoCompraConfiguration : IEntityTypeConfiguration<PagoCompra>
{
    public void Configure(EntityTypeBuilder<PagoCompra> builder)
    {
        builder.ToTable("PagosCompras");

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

        builder.HasOne<CompraInventario>()
            .WithMany(c => c.Pagos)
            .HasForeignKey(p => p.CompraId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.CompraId);
        builder.HasIndex(p => p.FechaPago);

        builder.HasQueryFilter(p => p.IsActive);
    }
}
