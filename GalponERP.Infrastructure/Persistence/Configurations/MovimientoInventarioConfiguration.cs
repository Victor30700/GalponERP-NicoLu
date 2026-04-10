using GalponERP.Domain.Entities;
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
            .HasPrecision(18, 2);

        builder.Property(m => m.Tipo)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.Fecha)
            .IsRequired();

        builder.HasOne<Producto>()
            .WithMany()
            .HasForeignKey(m => m.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Lote>()
            .WithMany()
            .HasForeignKey(m => m.LoteId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(m => m.IsActive);
    }
}
