using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Gastos.Queries.ObtenerGastos;

public class ObtenerGastosQueryHandler : IRequestHandler<ObtenerGastosQuery, IEnumerable<GastoOperativo>>
{
    private readonly IGastoOperativoRepository _gastoOperativoRepository;

    public ObtenerGastosQueryHandler(IGastoOperativoRepository gastoOperativoRepository)
    {
        _gastoOperativoRepository = gastoOperativoRepository;
    }

    public async Task<IEnumerable<GastoOperativo>> Handle(ObtenerGastosQuery request, CancellationToken cancellationToken)
    {
        if (request.LoteId.HasValue)
        {
            return await _gastoOperativoRepository.ObtenerPorLoteAsync(request.LoteId.Value);
        }

        if (request.GalponId.HasValue)
        {
            return await _gastoOperativoRepository.ObtenerPorGalponAsync(request.GalponId.Value);
        }

        return await _gastoOperativoRepository.ObtenerTodosAsync();
    }
}
