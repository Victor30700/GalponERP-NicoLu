using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Lotes.Queries.ObtenerReporteCierrePdf;

public class ObtenerReporteCierreLotePdfQueryHandler : IRequestHandler<ObtenerReporteCierreLotePdfQuery, byte[]>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IPdfService _pdfService;

    public ObtenerReporteCierreLotePdfQueryHandler(ILoteRepository loteRepository, IPdfService pdfService)
    {
        _loteRepository = loteRepository;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(ObtenerReporteCierreLotePdfQuery request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null) throw new Exception("Lote no encontrado.");

        // We pass the lote object directly for now, PdfService handles it as object
        return _pdfService.GenerarFichaLiquidacionLote(new {
            lote.Id,
            lote.GalponId,
            lote.FechaIngreso,
            lote.CantidadInicial,
            lote.CantidadActual,
            lote.MortalidadAcumulada,
            lote.Estado,
            lote.FCRFinal,
            CostoTotal = lote.CostoTotalFinal?.Monto,
            UtilidadNeta = lote.UtilidadNetaFinal?.Monto,
            lote.PorcentajeMortalidadFinal
        });
    }
}
