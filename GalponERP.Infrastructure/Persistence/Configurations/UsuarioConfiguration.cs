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

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Apellidos)
            .HasMaxLength(100);

        builder.Property(u => u.Direccion)
            .HasMaxLength(200);

        builder.Property(u => u.Profesion)
            .HasMaxLength(100);

        builder.Property(u => u.Rol)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.FechaNacimiento)
            .IsRequired();

        builder.HasQueryFilter(u => u.IsActive);
    }
}
