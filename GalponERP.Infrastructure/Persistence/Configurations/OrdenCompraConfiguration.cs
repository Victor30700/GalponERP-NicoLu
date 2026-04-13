using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class OrdenCompraConfiguration : IEntityTypeConfiguration<OrdenCompra>
{
    public void Configure(EntityTypeBuilder<OrdenCompra> builder)
    {
        builder.ToTable("OrdenesCompra");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Fecha)
            .IsRequired();

        builder.Property(o => o.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(o => o.Nota)
            .HasMaxLength(500);

        builder.ComplexProperty(o => o.Total, b =>
        {
            b.Property(m => m.Monto)
                .HasColumnName("TotalOrden")
                .HasPrecision(18, 2);
        });

        builder.HasOne(o => o.Proveedor)
            .WithMany()
            .HasForeignKey(o => o.ProveedorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrdenCompraId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(OrdenCompra.Items))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasQueryFilter(o => o.IsActive);
    }
}
