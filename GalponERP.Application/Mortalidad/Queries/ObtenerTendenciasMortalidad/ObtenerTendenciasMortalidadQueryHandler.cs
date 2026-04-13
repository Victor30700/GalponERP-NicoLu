using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Mortalidad.Queries.ObtenerTendenciasMortalidad;

public class ObtenerTendenciasMortalidadQueryHandler : IRequestHandler<ObtenerTendenciasMortalidadQuery, IEnumerable<TendenciaChartDto>>
{
    private readonly IMortalidadRepository _mortalidadRepository;
    private readonly ILoteRepository _loteRepository;

    public ObtenerTendenciasMortalidadQueryHandler(
        IMortalidadRepository mortalidadRepository,
        ILoteRepository loteRepository)
    {
        _mortalidadRepository = mortalidadRepository;
        _loteRepository = loteRepository;
    }

    public async Task<IEnumerable<TendenciaChartDto>> Handle(ObtenerTendenciasMortalidadQuery request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null) return Enumerable.Empty<TendenciaChartDto>();

        var mortalidad = await _mortalidadRepository.ObtenerPorLoteAsync(request.LoteId);

        var tendencias = mortalidad
            .Select(m => new
            {
                m.CantidadBajas,
                SemanaVida = ((int)(m.Fecha - lote.FechaIngreso).TotalDays / 7) + 1
            })
            .GroupBy(m => m.SemanaVida)
            .Select(g => new {
                Semana = g.Key,
                Dto = new TendenciaChartDto(
                    $"Semana {g.Key}",
                    g.Sum(x => x.CantidadBajas),
                    lote.CantidadInicial > 0 ? Math.Round((decimal)g.Sum(x => x.CantidadBajas) / lote.CantidadInicial * 100, 2) : 0,
                    g.Key,
                    0
                )
            })
            .OrderBy(x => x.Semana)
            .Select(x => x.Dto)
            .ToList();

        return tendencias;
    }
}
