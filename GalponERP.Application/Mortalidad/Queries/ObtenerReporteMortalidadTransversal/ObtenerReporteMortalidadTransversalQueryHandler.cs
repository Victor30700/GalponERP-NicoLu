using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Mortalidad.Queries.ObtenerReporteMortalidadTransversal;

public class ObtenerReporteMortalidadTransversalQueryHandler : IRequestHandler<ObtenerReporteMortalidadTransversalQuery, ReporteMortalidadTransversalDto>
{
    private readonly IMortalidadRepository _mortalidadRepository;
    private readonly ILoteRepository _loteRepository;

    public ObtenerReporteMortalidadTransversalQueryHandler(
        IMortalidadRepository mortalidadRepository,
        ILoteRepository loteRepository)
    {
        _mortalidadRepository = mortalidadRepository;
        _loteRepository = loteRepository;
    }

    public async Task<ReporteMortalidadTransversalDto> Handle(ObtenerReporteMortalidadTransversalQuery request, CancellationToken cancellationToken)
    {
        var mortalidades = await _mortalidadRepository.ObtenerPorRangoFechasAsync(request.Inicio, request.Fin);
        var lotes = (await _loteRepository.ObtenerTodosAsync()).ToDictionary(l => l.Id, l => l.Id.ToString().Substring(0, 8));

        int totalBajas = mortalidades.Sum(m => m.CantidadBajas);

        var porCausa = mortalidades
            .GroupBy(m => m.Causa)
            .Select(g => new MortalidadPorCausaDto(
                g.Key, 
                g.Sum(x => x.CantidadBajas),
                totalBajas > 0 ? (decimal)g.Sum(x => x.CantidadBajas) / totalBajas * 100 : 0))
            .OrderByDescending(c => c.Cantidad)
            .ToList();

        var detalle = mortalidades.Select(m => new MortalidadDetalleDto(
            m.Id, 
            m.Fecha, 
            lotes.ContainsKey(m.LoteId) ? lotes[m.LoteId] : "N/A", 
            m.CantidadBajas, 
            m.Causa)).OrderByDescending(d => d.Fecha).ToList();

        return new ReporteMortalidadTransversalDto(totalBajas, porCausa, detalle);
    }
}
