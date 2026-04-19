using GalponERP.Application.Interfaces;
using GalponERP.Application.Reporting.Dtos;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Lotes.Queries.ObtenerLiquidacionLotePdf;

public class ObtenerLiquidacionLotePdfQueryHandler : IRequestHandler<ObtenerLiquidacionLotePdfQuery, byte[]>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IVentaRepository _ventaRepository;
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IConfiguracionRepository _configRepository;
    private readonly IPdfService _pdfService;

    public ObtenerLiquidacionLotePdfQueryHandler(
        ILoteRepository loteRepository,
        IVentaRepository ventaRepository,
        IInventarioRepository inventarioRepository,
        IConfiguracionRepository configRepository,
        IPdfService pdfService)
    {
        _loteRepository = loteRepository;
        _ventaRepository = ventaRepository;
        _inventarioRepository = inventarioRepository;
        _configRepository = configRepository;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(ObtenerLiquidacionLotePdfQuery request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null) throw new Exception("Lote no encontrado.");

        var config = await _configRepository.ObtenerAsync();
        var ventas = await _ventaRepository.ObtenerPorLoteAsync(request.LoteId);
        var movimientos = await _inventarioRepository.ObtenerPorLoteIdAsync(request.LoteId);

        decimal ingresosVentas = ventas.Sum(v => v.Total.Monto);
        decimal costoInsumos = movimientos.Where(m => m.Tipo == TipoMovimiento.Salida).Sum(m => m.CostoTotal?.Monto ?? 0);
        decimal costoInicialAves = lote.CantidadInicial * lote.CostoUnitarioPollito.Monto;
        decimal costoTotal = costoInicialAves + costoInsumos;

        var ultimoPesaje = lote.Pesajes.OrderByDescending(p => p.Fecha).FirstOrDefault();
        decimal pesoPromedioFinal = ultimoPesaje?.PesoPromedioGramos ?? 0;

        // Cálculo aproximado de FCR si el lote no está cerrado
        decimal totalKgAlimento = movimientos
            .Where(m => m.Tipo == TipoMovimiento.Salida && m.Justificacion?.Contains("Alimento", StringComparison.OrdinalIgnoreCase) == true)
            .Sum(m => m.Cantidad);
        
        decimal totalPesoGanadoKg = (lote.CantidadActual * (pesoPromedioFinal / 1000m));
        decimal fcr = totalPesoGanadoKg > 0 ? (totalKgAlimento / totalPesoGanadoKg) : 0;

        var dto = new LiquidacionLoteReportDto
        {
            NombreEmpresa = config?.NombreEmpresa ?? "Pollos NicoLu",
            Nit = config?.Nit ?? "0000000-0",
            Direccion = config?.Direccion,
            Telefono = config?.Telefono,
            TituloReporte = "Liquidación de Lote (SAVCO-09)",
            NombreLote = lote.Nombre,
            NombreGalpon = lote.Galpon?.Nombre,
            FechaIngresoLote = lote.FechaIngreso,
            
            AvesIngresadas = lote.CantidadInicial,
            AvesFinales = lote.CantidadActual,
            TotalBajas = lote.MortalidadAcumulada,
            PorcentajeMortalidad = lote.CantidadInicial > 0 ? (decimal)lote.MortalidadAcumulada / lote.CantidadInicial * 100 : 0,
            
            PesoPromedioFinal = pesoPromedioFinal,
            FCRFinal = lote.FCRFinal ?? fcr,
            CostoTotal = lote.CostoTotalFinal?.Monto ?? costoTotal,
            IngresosVentas = ingresosVentas,
            UtilidadNeta = lote.UtilidadNetaFinal?.Monto ?? (ingresosVentas - costoTotal)
        };

        return _pdfService.GenerarLiquidacionLotePdf(dto);
    }
}
