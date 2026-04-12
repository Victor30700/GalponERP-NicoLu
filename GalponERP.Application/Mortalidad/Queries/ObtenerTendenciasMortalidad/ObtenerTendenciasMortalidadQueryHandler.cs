using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Mortalidad.Queries.ObtenerTendenciasMortalidad;

public class ObtenerTendenciasMortalidadQueryHandler : IRequestHandler<ObtenerTendenciasMortalidadQuery, TendenciasMortalidadResponse>
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

    public async Task<TendenciasMortalidadResponse> Handle(ObtenerTendenciasMortalidadQuery request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null) return null!;

        var mortalidad = await _mortalidadRepository.ObtenerPorLoteAsync(request.LoteId);

        var tendencias = mortalidad
            .Select(m => new
            {
                m.CantidadBajas,
                SemanaVida = ((int)(m.Fecha - lote.FechaIngreso).TotalDays / 7) + 1
            })
            .GroupBy(m => m.SemanaVida)
            .Select(g => new MortalidadPorSemanaDto(
                g.Key,
                g.Sum(x => x.CantidadBajas),
                lote.CantidadInicial > 0 
                    ? Math.Round((decimal)g.Sum(x => x.CantidadBajas) / lote.CantidadInicial * 100, 2)
                    : 0
            ))
            .OrderBy(t => t.SemanaVida)
            .ToList();

        return new TendenciasMortalidadResponse(lote.Id, tendencias);
    }
}
