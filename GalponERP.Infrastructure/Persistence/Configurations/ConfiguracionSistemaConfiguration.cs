using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalponERP.Infrastructure.Persistence.Configurations;

public class ConfiguracionSistemaConfiguration : IEntityTypeConfiguration<ConfiguracionSistema>
{
    public void Configure(EntityTypeBuilder<ConfiguracionSistema> builder)
    {
        builder.ToTable("ConfiguracionSistema");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.NombreEmpresa)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Nit)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.MonedaPorDefecto)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("USD");
    }
}
