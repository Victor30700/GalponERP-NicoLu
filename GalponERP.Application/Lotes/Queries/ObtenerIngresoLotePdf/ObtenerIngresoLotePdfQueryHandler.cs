using GalponERP.Application.Interfaces;
using GalponERP.Application.Reporting.Dtos;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Lotes.Queries.ObtenerIngresoLotePdf;

public class ObtenerIngresoLotePdfQueryHandler : IRequestHandler<ObtenerIngresoLotePdfQuery, byte[]>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IConfiguracionRepository _configRepository;
    private readonly IPdfService _pdfService;

    public ObtenerIngresoLotePdfQueryHandler(
        ILoteRepository loteRepository, 
        IConfiguracionRepository configRepository, 
        IPdfService pdfService)
    {
        _loteRepository = loteRepository;
        _configRepository = configRepository;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(ObtenerIngresoLotePdfQuery request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null) throw new Exception("Lote no encontrado.");

        var config = await _configRepository.ObtenerAsync();

        var dto = new IngresoLoteReportDto
        {
            NombreEmpresa = config?.NombreEmpresa ?? "Pollos NicoLu",
            Nit = config?.Nit ?? "0000000-0",
            Direccion = config?.Direccion,
            Telefono = config?.Telefono,
            TituloReporte = "Registro de Ingreso de Lote (SAVCO-01)",
            NombreLote = lote.Nombre,
            NombreGalpon = lote.Galpon?.Nombre ?? "N/A",
            FechaIngresoLote = lote.FechaIngreso,
            CantidadInicial = lote.CantidadInicial,
            Raza = "Cobb 500", // Estándar por defecto
            PesoPromedioIngreso = 42.5m, // Valor estándar de pollito de un día en gramos
            Observaciones = "Registro inicial generado automáticamente por el sistema."
        };

        return _pdfService.GenerarRegistroIngresoLotePdf(dto);
    }
}
