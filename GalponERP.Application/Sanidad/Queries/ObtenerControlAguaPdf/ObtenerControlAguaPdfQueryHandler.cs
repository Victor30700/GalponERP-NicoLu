using GalponERP.Application.Interfaces;
using GalponERP.Application.Reporting.Dtos;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Sanidad.Queries.ObtenerControlAguaPdf;

public class ObtenerControlAguaPdfQueryHandler : IRequestHandler<ObtenerControlAguaPdfQuery, byte[]>
{
    private readonly IRegistroBienestarRepository _bienestarRepository;
    private readonly IConfiguracionRepository _configRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IPdfService _pdfService;

    public ObtenerControlAguaPdfQueryHandler(
        IRegistroBienestarRepository bienestarRepository,
        IConfiguracionRepository configRepository,
        ILoteRepository loteRepository,
        IPdfService pdfService)
    {
        _bienestarRepository = bienestarRepository;
        _configRepository = configRepository;
        _loteRepository = loteRepository;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(ObtenerControlAguaPdfQuery request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null) throw new Exception("Lote no encontrado.");

        var config = await _configRepository.ObtenerAsync();
        var registros = await _bienestarRepository.ObtenerPorLoteAsync(request.LoteId);
        
        // Filtrar por el mes solicitado
        var registrosMes = registros
            .Where(r => r.Fecha.Month == request.Mes.Month && r.Fecha.Year == request.Mes.Year)
            .OrderBy(r => r.Fecha)
            .ToList();

        var dto = new ControlAguaReportDto
        {
            NombreEmpresa = config?.NombreEmpresa ?? "Pollos NicoLu",
            Nit = config?.Nit ?? "0000000-0",
            Direccion = config?.Direccion,
            Telefono = config?.Telefono,
            TituloReporte = "Control de Agua y Cloración (SAVCO-07)",
            NombreLote = lote.Nombre,
            NombreGalpon = lote.Galpon?.Nombre,
            FechaIngresoLote = lote.FechaIngreso,
            MesAnio = request.Mes.ToString("MMMM yyyy"),
            Registros = registrosMes.Select(r => new RegistroAguaDto
            {
                Fecha = r.Fecha,
                CloroPpm = r.CloroPpm ?? 0,
                Ph = r.Ph ?? 0,
                Temperatura = r.Temperatura ?? 0
            }).ToList()
        };

        return _pdfService.GenerarControlAguaPdf(dto);
    }
}
