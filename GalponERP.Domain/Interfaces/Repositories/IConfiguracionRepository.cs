using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IConfiguracionRepository
{
    Task<ConfiguracionSistema?> ObtenerAsync();
    void Actualizar(ConfiguracionSistema configuracion);
    void Agregar(ConfiguracionSistema configuracion);
}
