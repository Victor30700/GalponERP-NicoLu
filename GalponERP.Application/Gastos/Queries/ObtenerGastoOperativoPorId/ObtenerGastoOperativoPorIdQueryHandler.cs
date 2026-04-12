using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Gastos.Queries.ObtenerGastoOperativoPorId;

public class ObtenerGastoOperativoPorIdQueryHandler : IRequestHandler<ObtenerGastoOperativoPorIdQuery, GastoOperativoResponse?>
{
    private readonly IGastoOperativoRepository _gastoOperativoRepository;

    public ObtenerGastoOperativoPorIdQueryHandler(IGastoOperativoRepository gastoOperativoRepository)
    {
        _gastoOperativoRepository = gastoOperativoRepository;
    }

    public async Task<GastoOperativoResponse?> Handle(ObtenerGastoOperativoPorIdQuery request, CancellationToken cancellationToken)
    {
        var gasto = await _gastoOperativoRepository.ObtenerPorIdAsync(request.Id);

        if (gasto == null)
        {
            return null;
        }

        return new GastoOperativoResponse(
            gasto.Id,
            gasto.GalponId,
            gasto.LoteId,
            gasto.Descripcion,
            gasto.Monto.Monto,
            gasto.Fecha,
            gasto.TipoGasto,
            gasto.UsuarioId);
    }
}
