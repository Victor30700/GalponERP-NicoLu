using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class OrdenCompraItemConfiguration : IEntityTypeConfiguration<OrdenCompraItem>
{
    public void Configure(EntityTypeBuilder<OrdenCompraItem> builder)
    {
        builder.ToTable("OrdenesCompraItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Cantidad)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.ComplexProperty(i => i.PrecioUnitario, b =>
        {
            b.Property(m => m.Monto)
                .HasColumnName("PrecioUnitario")
                .HasPrecision(18, 2);
        });

        builder.ComplexProperty(i => i.Total, b =>
        {
            b.Property(m => m.Monto)
                .HasColumnName("TotalItem")
                .HasPrecision(18, 2);
        });

        builder.HasOne(i => i.Producto)
            .WithMany()
            .HasForeignKey(i => i.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(i => i.IsActive);
    }
}
