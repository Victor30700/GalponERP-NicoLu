using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class AuditoriaLogConfiguration : IEntityTypeConfiguration<AuditoriaLog>
{
    public void Configure(EntityTypeBuilder<AuditoriaLog> builder)
    {
        builder.ToTable("AuditoriaLogs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.UsuarioId)
            .IsRequired();

        builder.Property(a => a.Accion)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Entidad)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntidadId)
            .IsRequired();

        builder.Property(a => a.Fecha)
            .IsRequired();

        builder.Property(a => a.DetallesJSON)
            .HasColumnType("jsonb"); // PostgreSQL jsonb para búsqueda eficiente
    }
}
