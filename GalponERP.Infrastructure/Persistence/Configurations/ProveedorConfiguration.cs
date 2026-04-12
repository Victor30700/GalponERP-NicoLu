using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
{
    public void Configure(EntityTypeBuilder<Proveedor> builder)
    {
        builder.ToTable("Proveedores");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.RazonSocial)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.NitRuc)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Telefono)
            .HasMaxLength(20);

        builder.Property(p => p.Email)
            .HasMaxLength(100);

        builder.Property(p => p.Direccion)
            .HasMaxLength(300);

        builder.HasIndex(p => p.NitRuc).IsUnique();
        
        builder.HasQueryFilter(p => p.IsActive);
    }
}
