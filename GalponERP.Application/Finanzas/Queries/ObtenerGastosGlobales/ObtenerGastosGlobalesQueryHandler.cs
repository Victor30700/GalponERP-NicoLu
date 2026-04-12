using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Finanzas.Queries.ObtenerGastosGlobales;

public class ObtenerGastosGlobalesQueryHandler : IRequestHandler<ObtenerGastosGlobalesQuery, IEnumerable<GastoGlobalResponse>>
{
    private readonly IGastoOperativoRepository _gastoRepository;

    public ObtenerGastosGlobalesQueryHandler(IGastoOperativoRepository gastoRepository)
    {
        _gastoRepository = gastoRepository;
    }

    public async Task<IEnumerable<GastoGlobalResponse>> Handle(ObtenerGastosGlobalesQuery request, CancellationToken cancellationToken)
    {
        var gastos = await _gastoRepository.ObtenerTodosAsync();

        // Aplicamos filtros en memoria (para reportes administrativos)
        var query = gastos.AsQueryable();

        if (request.FechaInicio.HasValue)
        {
            var inicio = DateTime.SpecifyKind(request.FechaInicio.Value, DateTimeKind.Utc);
            query = query.Where(g => g.Fecha >= inicio);
        }

        if (request.FechaFin.HasValue)
        {
            var fin = DateTime.SpecifyKind(request.FechaFin.Value, DateTimeKind.Utc);
            query = query.Where(g => g.Fecha <= fin);
        }

        if (!string.IsNullOrWhiteSpace(request.Categoria))
        {
            query = query.Where(g => g.TipoGasto.ToString().Equals(request.Categoria, StringComparison.OrdinalIgnoreCase));
        }

        return query
            .OrderByDescending(g => g.Fecha)
            .Select(g => new GastoGlobalResponse(
                g.Id,
                g.GalponId,
                g.LoteId,
                g.Descripcion,
                g.Monto.Monto,
                g.Fecha,
                g.TipoGasto.ToString(),
                g.UsuarioId
            ));
    }
}
