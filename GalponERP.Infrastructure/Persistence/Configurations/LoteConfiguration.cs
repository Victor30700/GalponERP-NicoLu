using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class LoteConfiguration : IEntityTypeConfiguration<Lote>
{
    public void Configure(EntityTypeBuilder<Lote> builder)
    {
        builder.ToTable("Lotes");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.FechaIngreso)
            .IsRequired();

        builder.Property(l => l.CantidadInicial)
            .IsRequired();

        builder.Property(l => l.CantidadActual)
            .IsRequired();

        builder.Property(l => l.MortalidadAcumulada)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(l => l.PollosVendidos)
            .IsRequired()
            .HasDefaultValue(0);

        builder.ComplexProperty(l => l.CostoUnitarioPollito, b =>
        {
            b.Property(m => m.Monto)
                .HasColumnName("CostoUnitarioPollito")
                .HasPrecision(18, 2);
        });

        builder.Property(l => l.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasQueryFilter(l => l.IsActive);
    }
}
