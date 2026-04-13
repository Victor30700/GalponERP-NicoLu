using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class CalendarioSanitarioConfiguration : IEntityTypeConfiguration<CalendarioSanitario>
{
    public void Configure(EntityTypeBuilder<CalendarioSanitario> builder)
    {
        builder.ToTable("CalendarioSanitario");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.LoteId)
            .IsRequired();

        builder.Property(c => c.DiaDeAplicacion)
            .IsRequired();

        builder.Property(c => c.DescripcionTratamiento)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(c => c.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.Tipo)
            .HasConversion<string>()
            .HasMaxLength(30)
            .HasDefaultValue(TipoActividad.Otros)
            .HasSentinel((TipoActividad)0);

        builder.Property(c => c.EsManual)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.Justificacion)
            .HasMaxLength(500);

        builder.HasOne<Producto>()
            .WithMany()
            .HasForeignKey(c => c.ProductoIdRecomendado)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Lote>()
            .WithMany()
            .HasForeignKey(c => c.LoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.LoteId);

        builder.HasQueryFilter(c => c.IsActive);
    }
}
