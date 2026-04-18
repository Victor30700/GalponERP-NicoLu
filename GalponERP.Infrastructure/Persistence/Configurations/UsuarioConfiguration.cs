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

        builder.Property(u => u.Telefono)
            .HasMaxLength(20);

        builder.HasIndex(u => u.Telefono)
            .IsUnique()
            .HasFilter("\"Telefono\" IS NOT NULL");

        builder.Property(u => u.WhatsAppNumero)
            .HasMaxLength(20);

        builder.Property(u => u.CodigoVinculacion)
            .HasMaxLength(10);

        builder.Property(u => u.FechaExpiracionCodigo);

        builder.Property(u => u.Rol)
            .IsRequired();

        builder.Property(u => u.Active)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(u => u.FechaNacimiento)
            .IsRequired();

        builder.HasQueryFilter(u => u.IsActive);
    }
}
