using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class CompraInventarioConfiguration : IEntityTypeConfiguration<CompraInventario>
{
    public void Configure(EntityTypeBuilder<CompraInventario> builder)
    {
        builder.ToTable("ComprasInventario");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Fecha)
            .IsRequired();

        builder.ComplexProperty(c => c.Total, b =>
        {
            b.Property(p => p.Monto)
                .HasColumnName("TotalCompra")
                .HasPrecision(18, 2);
        });

        builder.ComplexProperty(c => c.TotalPagado, b =>
        {
            b.Property(p => p.Monto)
                .HasColumnName("TotalPagado")
                .HasPrecision(18, 2);
        });

        builder.Property(c => c.EstadoPago)
            .IsRequired()
            .HasDefaultValue(EstadoPago.Pendiente);

        builder.HasOne<Proveedor>()
            .WithMany()
            .HasForeignKey(c => c.ProveedorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Pagos)
            .WithOne()
            .HasForeignKey(p => p.CompraId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(CompraInventario.Pagos))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(c => c.ProveedorId);
        builder.HasIndex(c => c.Fecha);

        builder.HasQueryFilter(c => c.IsActive);
    }
}
