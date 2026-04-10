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

        builder.Property(p => p.Tipo)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(p => p.UnidadMedida)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasQueryFilter(p => p.IsActive);
    }
}
