using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class PlantillaSanitariaConfiguration : IEntityTypeConfiguration<PlantillaSanitaria>
{
    public void Configure(EntityTypeBuilder<PlantillaSanitaria> builder)
    {
        builder.ToTable("PlantillasSanitarias");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Descripcion)
            .HasMaxLength(500);

        builder.HasMany(p => p.Actividades)
            .WithOne()
            .HasForeignKey(a => a.PlantillaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(PlantillaSanitaria.Actividades))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasQueryFilter(p => p.IsActive);
    }
}

public class ActividadPlantillaConfiguration : IEntityTypeConfiguration<ActividadPlantilla>
{
    public void Configure(EntityTypeBuilder<ActividadPlantilla> builder)
    {
        builder.ToTable("ActividadesPlantillas");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Descripcion)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.TipoActividad)
            .IsRequired();

        builder.Property(a => a.DiaDeAplicacion)
            .IsRequired();

        builder.HasOne<Producto>()
            .WithMany()
            .HasForeignKey(a => a.ProductoIdRecomendado)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.PlantillaId);

        builder.HasQueryFilter(a => a.IsActive);
    }
}
