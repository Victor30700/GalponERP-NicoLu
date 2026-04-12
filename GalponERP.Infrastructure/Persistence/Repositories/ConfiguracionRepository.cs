using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class ConfiguracionRepository : IConfiguracionRepository
{
    private readonly GalponDbContext _context;

    public ConfiguracionRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<ConfiguracionSistema?> ObtenerAsync()
    {
        return await _context.Configuracion.FirstOrDefaultAsync();
    }

    public void Agregar(ConfiguracionSistema configuracion)
    {
        _context.Configuracion.Add(configuracion);
    }

    public void Actualizar(ConfiguracionSistema configuracion)
    {
        _context.Configuracion.Update(configuracion);
    }
}
