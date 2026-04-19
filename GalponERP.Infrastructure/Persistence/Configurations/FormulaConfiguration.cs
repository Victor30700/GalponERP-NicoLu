using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class FormulaConfiguration : IEntityTypeConfiguration<Formula>
{
    public void Configure(EntityTypeBuilder<Formula> builder)
    {
        builder.ToTable("Formulas");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Etapa)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(f => f.CantidadBase)
            .HasPrecision(18, 2);

        builder.HasMany(f => f.Detalles)
            .WithOne(d => d.Formula)
            .HasForeignKey(d => d.FormulaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
