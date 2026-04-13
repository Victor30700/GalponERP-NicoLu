using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class MensajeChatConfiguration : IEntityTypeConfiguration<MensajeChat>
{
    public void Configure(EntityTypeBuilder<MensajeChat> builder)
    {
        builder.ToTable("MensajesChat");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.ConversacionId)
            .IsRequired();

        builder.Property(m => m.Rol)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(m => m.Contenido)
            .IsRequired();

        builder.Property(m => m.Fecha)
            .IsRequired();
    }
}
