using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class GastoOperativoConfiguration : IEntityTypeConfiguration<GastoOperativo>
{
    public void Configure(EntityTypeBuilder<GastoOperativo> builder)
    {
        builder.ToTable("GastosOperativos");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Descripcion)
            .IsRequired()
            .HasMaxLength(250);

        builder.ComplexProperty(g => g.Monto, b =>
        {
            b.Property(m => m.Monto)
                .HasColumnName("Monto")
                .HasPrecision(18, 2);
        });

        builder.Property(g => g.Fecha)
            .IsRequired();

        builder.HasOne<Galpon>()
            .WithMany()
            .HasForeignKey(g => g.GalponId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Lote>()
            .WithMany()
            .HasForeignKey(g => g.LoteId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(g => g.IsActive);
    }
}
