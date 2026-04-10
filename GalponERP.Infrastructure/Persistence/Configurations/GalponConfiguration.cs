using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class GalponConfiguration : IEntityTypeConfiguration<Galpon>
{
    public void Configure(EntityTypeBuilder<Galpon> builder)
    {
        builder.ToTable("Galpones");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Capacidad)
            .IsRequired();

        builder.Property(g => g.Ubicacion)
            .HasMaxLength(255);

        builder.HasQueryFilter(g => g.IsActive);
    }
}
