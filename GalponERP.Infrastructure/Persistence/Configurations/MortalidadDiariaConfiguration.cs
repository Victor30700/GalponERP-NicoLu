using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class MortalidadDiariaConfiguration : IEntityTypeConfiguration<MortalidadDiaria>
{
    public void Configure(EntityTypeBuilder<MortalidadDiaria> builder)
    {
        builder.ToTable("Mortalidades");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Fecha)
            .IsRequired();

        builder.Property(m => m.CantidadBajas)
            .IsRequired();

        builder.HasOne<Lote>()
            .WithMany()
            .HasForeignKey(m => m.LoteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(m => m.IsActive);
    }
}
