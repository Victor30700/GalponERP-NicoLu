using GalponERP.Domain.Entities;

namespace GalponERP.Application.Interfaces;

public interface IPdfService
{
    byte[] GenerarFichaLiquidacionLote(object datos, ConfiguracionSistema? config = null);
}
