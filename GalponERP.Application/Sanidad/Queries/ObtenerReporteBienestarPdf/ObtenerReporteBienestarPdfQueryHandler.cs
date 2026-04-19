using GalponERP.Application.Interfaces;
using GalponERP.Application.Reporting.Dtos;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Sanidad.Queries.ObtenerReporteBienestarPdf;

public class ObtenerReporteBienestarPdfQueryHandler : IRequestHandler<ObtenerReporteBienestarPdfQuery, byte[]>
{
    private readonly IGalponDbContext _context;
    private readonly IConfiguracionRepository _configRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IPdfService _pdfService;

    public ObtenerReporteBienestarPdfQueryHandler(
        IGalponDbContext context,
        IConfiguracionRepository configRepository,
        ILoteRepository loteRepository,
        IPdfService pdfService)
    {
        _context = context;
        _configRepository = configRepository;
        _loteRepository = loteRepository;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(ObtenerReporteBienestarPdfQuery request, CancellationToken cancellationToken)
    {
        var registro = await _context.ObtenerEntidadPorIdAsync<RegistroBienestar>(request.RegistroId, cancellationToken);
        if (registro == null) throw new Exception("Registro de bienestar no encontrado.");

        var lote = await _loteRepository.ObtenerPorIdAsync(registro.LoteId);
        var config = await _configRepository.ObtenerAsync();

        var dto = new BienestarReportDto
        {
            NombreEmpresa = config?.NombreEmpresa ?? "Pollos NicoLu",
            Nit = config?.Nit ?? "0000000-0",
            Direccion = config?.Direccion,
            Telefono = config?.Telefono,
            TituloReporte = "Registro de Bienestar Animal (SAVCO-06)",
            NombreLote = lote?.Nombre,
            NombreGalpon = lote?.Galpon?.Nombre,
            FechaIngresoLote = lote?.FechaIngreso,
            FechaRegistro = registro.Fecha,
            Temperatura = registro.Temperatura,
            Humedad = registro.Humedad,
            ConsumoAgua = registro.ConsumoAgua,
            Observaciones = registro.Observaciones,
            Checklist = ObtenerChecklistEstandar()
        };

        return _pdfService.GenerarReporteBienestarPdf(dto);
    }

    private List<ItemBienestarDto> ObtenerChecklistEstandar()
    {
        return new List<ItemBienestarDto>
        {
            new() { Concepto = "Ausencia de hambre prolongada", Cumple = true },
            new() { Concepto = "Ausencia de sed prolongada", Cumple = true },
            new() { Concepto = "Confort durante el descanso", Cumple = true },
            new() { Concepto = "Confort térmico (Temperatura adecuada)", Cumple = true },
            new() { Concepto = "Facilidad de movimiento", Cumple = true },
            new() { Concepto = "Ausencia de lesiones", Cumple = true },
            new() { Concepto = "Ausencia de enfermedades", Cumple = true },
            new() { Concepto = "Ausencia de dolor inducido por manejo", Cumple = true },
            new() { Concepto = "Expresión de comportamientos sociales", Cumple = true },
            new() { Concepto = "Expresión de otros comportamientos", Cumple = true },
            new() { Concepto = "Buena relación humano-animal", Cumple = true },
            new() { Concepto = "Ausencia de miedo general", Cumple = true }
        };
    }
}
