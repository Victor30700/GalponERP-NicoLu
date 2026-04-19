using GalponERP.Application.Interfaces;
using GalponERP.Application.Reporting.Dtos;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Sanidad.Queries.ObtenerCalendarioSanitarioPdf;

public class ObtenerCalendarioSanitarioPdfQueryHandler : IRequestHandler<ObtenerCalendarioSanitarioPdfQuery, byte[]>
{
    private readonly ILoteRepository _loteRepository;
    private readonly ICalendarioSanitarioRepository _calendarioRepository;
    private readonly IConfiguracionRepository _configRepository;
    private readonly IPdfService _pdfService;

    public ObtenerCalendarioSanitarioPdfQueryHandler(
        ILoteRepository loteRepository,
        ICalendarioSanitarioRepository calendarioRepository,
        IConfiguracionRepository configRepository,
        IPdfService pdfService)
    {
        _loteRepository = loteRepository;
        _calendarioRepository = calendarioRepository;
        _configRepository = configRepository;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(ObtenerCalendarioSanitarioPdfQuery request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null) throw new Exception("Lote no encontrado.");

        var config = await _configRepository.ObtenerAsync();
        var eventos = await _calendarioRepository.ObtenerPorLoteIdAsync(request.LoteId);
        
        var dto = new SanidadReportDto
        {
            NombreEmpresa = config?.NombreEmpresa ?? "Pollos NicoLu",
            Nit = config?.Nit ?? "0000000-0",
            Direccion = config?.Direccion,
            Telefono = config?.Telefono,
            TituloReporte = "Programa de Vacunación y Sanidad (SAVCO-05)",
            NombreLote = lote.Nombre,
            NombreGalpon = lote.Galpon?.Nombre ?? "N/A",
            FechaIngresoLote = lote.FechaIngreso,
            Eventos = eventos.OrderBy(x => x.DiaDeAplicacion).Select(e => new EventoSanitarioDto
            {
                Fecha = lote.FechaIngreso.AddDays(e.DiaDeAplicacion),
                Actividad = e.DescripcionTratamiento,
                Producto = "Ver Plan",
                Dosis = e.CantidadRecomendada.ToString("N2"),
                ViaAplicacion = e.Tipo.ToString(),
                Responsable = "Operario Encargado"
            }).ToList()
        };

        return _pdfService.GenerarCalendarioSanitarioPdf(dto);
    }
}
