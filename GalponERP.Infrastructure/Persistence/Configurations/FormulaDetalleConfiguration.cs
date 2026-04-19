using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class FormulaDetalleConfiguration : IEntityTypeConfiguration<FormulaDetalle>
{
    public void Configure(EntityTypeBuilder<FormulaDetalle> builder)
    {
        builder.ToTable("FormulaDetalles");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.CantidadPorBase)
            .HasPrecision(18, 4);

        builder.HasOne(d => d.Producto)
            .WithMany()
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
