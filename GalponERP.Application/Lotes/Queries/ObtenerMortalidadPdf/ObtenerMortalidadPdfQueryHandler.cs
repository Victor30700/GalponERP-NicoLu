using GalponERP.Application.Interfaces;
using GalponERP.Application.Reporting.Dtos;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Lotes.Queries.ObtenerMortalidadPdf;

public class ObtenerMortalidadPdfQueryHandler : IRequestHandler<ObtenerMortalidadPdfQuery, byte[]>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IMortalidadRepository _mortalidadRepository;
    private readonly IConfiguracionRepository _configRepository;
    private readonly IPdfService _pdfService;

    public ObtenerMortalidadPdfQueryHandler(
        ILoteRepository loteRepository,
        IMortalidadRepository mortalidadRepository,
        IConfiguracionRepository configRepository,
        IPdfService pdfService)
    {
        _loteRepository = loteRepository;
        _mortalidadRepository = mortalidadRepository;
        _configRepository = configRepository;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(ObtenerMortalidadPdfQuery request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null) throw new Exception("Lote no encontrado.");

        var config = await _configRepository.ObtenerAsync();
        var historial = await _mortalidadRepository.ObtenerPorLoteAsync(request.LoteId);
        
        var detalles = new List<MortalidadDetalleDto>();
        int acumulado = 0;

        foreach (var m in historial.OrderBy(x => x.Fecha))
        {
            acumulado += m.CantidadBajas;
            detalles.Add(new MortalidadDetalleDto
            {
                Fecha = m.Fecha,
                Cantidad = m.CantidadBajas,
                Causa = m.Causa ?? "No especificada",
                Acumulado = acumulado
            });
        }

        var dto = new MortalidadReportDto
        {
            NombreEmpresa = config?.NombreEmpresa ?? "Pollos NicoLu",
            Nit = config?.Nit ?? "0000000-0",
            Direccion = config?.Direccion,
            Telefono = config?.Telefono,
            TituloReporte = "Reporte de Mortalidad Diaria y Acumulada (SAVCO-02)",
            NombreLote = lote.Nombre,
            NombreGalpon = lote.Galpon?.Nombre ?? "N/A",
            FechaIngresoLote = lote.FechaIngreso,
            Detalles = detalles,
            TotalBajas = lote.MortalidadAcumulada,
            PorcentajeMortalidad = lote.CantidadInicial > 0 
                ? (decimal)lote.MortalidadAcumulada / lote.CantidadInicial * 100 
                : 0
        };

        return _pdfService.GenerarReporteMortalidadPdf(dto);
    }
}
