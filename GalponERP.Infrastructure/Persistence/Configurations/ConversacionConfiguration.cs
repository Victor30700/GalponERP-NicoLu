using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class ConversacionConfiguration : IEntityTypeConfiguration<Conversacion>
{
    public void Configure(EntityTypeBuilder<Conversacion> builder)
    {
        builder.ToTable("Conversaciones");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.UsuarioId)
            .IsRequired();

        builder.Property(c => c.FechaInicio)
            .IsRequired();

        builder.Property(c => c.Estado)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasMany(c => c.Mensajes)
            .WithOne(m => m.Conversacion)
            .HasForeignKey(m => m.ConversacionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
