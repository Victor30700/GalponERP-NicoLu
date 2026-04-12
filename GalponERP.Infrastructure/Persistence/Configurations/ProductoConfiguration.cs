using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("Productos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.EquivalenciaEnKg)
            .HasPrecision(18, 4);

        builder.Property(p => p.UmbralMinimo)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(p => p.CostoUnitarioActual)
            .HasPrecision(18, 4)
            .HasDefaultValue(0);

        builder.HasOne(p => p.Categoria)
            .WithMany()
            .HasForeignKey(p => p.CategoriaProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Unidad)
            .WithMany()
            .HasForeignKey(p => p.UnidadMedidaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(p => p.IsActive);
    }
}
