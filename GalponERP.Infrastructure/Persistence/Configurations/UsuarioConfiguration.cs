using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirebaseUid)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasIndex(u => u.FirebaseUid)
            .IsUnique();

        builder.Property(u => u.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.Rol)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasQueryFilter(u => u.IsActive);
    }
}
