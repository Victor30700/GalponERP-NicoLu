using GalponERP.Application.Interfaces;
using GalponERP.Application.Reporting.Dtos;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Lotes.Queries.ObtenerFichaSemanalPdf;

public class ObtenerFichaSemanalPdfQueryHandler : IRequestHandler<ObtenerFichaSemanalPdfQuery, byte[]>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IPesajeLoteRepository _pesajeRepository;
    private readonly IConfiguracionRepository _configRepository;
    private readonly IPdfService _pdfService;

    public ObtenerFichaSemanalPdfQueryHandler(
        ILoteRepository loteRepository,
        IPesajeLoteRepository pesajeRepository,
        IConfiguracionRepository configRepository,
        IPdfService pdfService)
    {
        _loteRepository = loteRepository;
        _pesajeRepository = pesajeRepository;
        _configRepository = configRepository;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(ObtenerFichaSemanalPdfQuery request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null) throw new Exception("Lote no encontrado.");

        var config = await _configRepository.ObtenerAsync();
        var pesajes = await _pesajeRepository.ObtenerPorLoteIdAsync(request.LoteId);
        
        var semanas = new List<SemanaCrecimientoDto>();
        
        // Agrupar pesajes por semana de vida (cada 7 días desde FechaIngreso)
        var pesajesAgrupados = pesajes
            .GroupBy(p => (p.Fecha.Date - lote.FechaIngreso.Date).Days / 7 + 1)
            .OrderBy(g => g.Key);

        foreach (var grupo in pesajesAgrupados)
        {
            int semana = grupo.Key;
            decimal pesoReal = grupo.OrderByDescending(p => p.Fecha).First().PesoPromedioGramos;
            decimal pesoEstandar = ObtenerPesoEstandarGramos(semana * 7);
            decimal desviacion = pesoReal - pesoEstandar;
            decimal porcentajeDesviacion = pesoEstandar > 0 ? (desviacion / pesoEstandar) * 100 : 0;

            semanas.Add(new SemanaCrecimientoDto
            {
                NumeroSemana = semana,
                PesoRealGramos = pesoReal,
                PesoEstandarGramos = pesoEstandar,
                Desviacion = desviacion,
                PorcentajeDesviacion = porcentajeDesviacion
            });
        }

        var dto = new FichaSemanalReportDto
        {
            NombreEmpresa = config?.NombreEmpresa ?? "Pollos NicoLu",
            Nit = config?.Nit ?? "0000000-0",
            Direccion = config?.Direccion,
            Telefono = config?.Telefono,
            TituloReporte = "Ficha Semanal de Pesaje y Productividad (SAVCO-03)",
            NombreLote = lote.Nombre,
            NombreGalpon = lote.Galpon?.Nombre ?? "N/A",
            FechaIngresoLote = lote.FechaIngreso,
            Semanas = semanas
        };

        return _pdfService.GenerarFichaSemanalPdf(dto);
    }

    private decimal ObtenerPesoEstandarGramos(int dia)
    {
        // Estándar Cobb 500 (Oficial)
        if (dia <= 7) return 190;
        if (dia <= 14) return 480;
        if (dia <= 21) return 950;
        if (dia <= 28) return 1600;
        if (dia <= 35) return 2350;
        if (dia <= 42) return 3100;
        if (dia <= 49) return 3700;
        return 4200;
    }
}
