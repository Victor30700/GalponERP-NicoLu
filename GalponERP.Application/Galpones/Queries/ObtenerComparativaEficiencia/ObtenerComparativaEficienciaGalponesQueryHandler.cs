using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Galpones.Queries.ObtenerComparativaEficiencia;

public class ObtenerComparativaEficienciaGalponesQueryHandler : IRequestHandler<ObtenerComparativaEficienciaGalponesQuery, List<EficienciaGalponDto>>
{
    private readonly IGalponRepository _galponRepository;
    private readonly ILoteRepository _loteRepository;

    public ObtenerComparativaEficienciaGalponesQueryHandler(
        IGalponRepository galponRepository,
        ILoteRepository loteRepository)
    {
        _galponRepository = galponRepository;
        _loteRepository = loteRepository;
    }

    public async Task<List<EficienciaGalponDto>> Handle(ObtenerComparativaEficienciaGalponesQuery request, CancellationToken cancellationToken)
    {
        var galpones = await _galponRepository.ObtenerTodosAsync();
        var lotes = await _loteRepository.ObtenerTodosAsync();

        var comparativa = galpones.Select(g => {
            var lotesDelGalpon = lotes.Where(l => l.GalponId == g.Id && l.Estado == EstadoLote.Cerrado).ToList();
            
            return new EficienciaGalponDto(
                g.Id,
                g.Nombre,
                lotesDelGalpon.Count,
                lotesDelGalpon.Any() ? lotesDelGalpon.Average(l => l.PorcentajeMortalidadFinal ?? 0) : 0,
                lotesDelGalpon.Any() ? lotesDelGalpon.Average(l => l.FCRFinal ?? 0) : 0,
                lotesDelGalpon.Any() ? lotesDelGalpon.Sum(l => l.UtilidadNetaFinal?.Monto ?? 0) : 0
            );
        }).OrderByDescending(x => x.UtilidadTotalAcumulada).ToList();

        return comparativa;
    }
}
