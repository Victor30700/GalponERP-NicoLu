using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Ruc)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Direccion)
            .HasMaxLength(500);

        builder.Property(c => c.Telefono)
            .HasMaxLength(50);

        builder.HasQueryFilter(c => c.IsActive);
    }
}
