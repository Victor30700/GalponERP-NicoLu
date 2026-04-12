using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Finanzas.Queries.ObtenerGastosPorCategoria;

public class ObtenerGastosPorCategoriaQueryHandler : IRequestHandler<ObtenerGastosPorCategoriaQuery, IEnumerable<GastoCategoriaResponse>>
{
    private readonly IGastoOperativoRepository _gastoRepository;

    public ObtenerGastosPorCategoriaQueryHandler(IGastoOperativoRepository gastoRepository)
    {
        _gastoRepository = gastoRepository;
    }

    public async Task<IEnumerable<GastoCategoriaResponse>> Handle(ObtenerGastosPorCategoriaQuery request, CancellationToken cancellationToken)
    {
        var gastos = await _gastoRepository.ObtenerPorRangoFechasAsync(request.Inicio, request.Fin);

        var agrupados = gastos
            .GroupBy(g => g.TipoGasto)
            .Select(g => new GastoCategoriaResponse(
                g.Key,
                g.Sum(x => x.Monto.Monto)))
            .OrderByDescending(g => g.Total)
            .ToList();

        return agrupados;
    }
}
