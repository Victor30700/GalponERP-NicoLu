using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Lotes.Queries.ObtenerDetalleLote;

public record ObtenerDetalleLoteQuery(Guid LoteId) : IRequest<LoteDetalleResponse>;

public record LoteDetalleResponse(
    Guid Id,
    string Nombre,
    string NombreLote, // Compatibilidad frontend
    Guid GalponId,
    string NombreGalpon,
    string GalponNombre,
    DateTime FechaInicio,
    DateTime FechaIngreso,
    int CantidadInicial,
    int AvesVivas, // Compatibilidad frontend
    int CantidadActual,
    int MortalidadTotal, // Compatibilidad frontend
    int MortalidadAcumulada,
    decimal MortalidadPorcentaje,
    int PollosVendidos,
    decimal CostoUnitarioPollito,
    int EdadSemanas,
    string Estado,
    decimal TotalVentas,
    decimal TotalGastos,
    decimal CostoTotalAcumulado,
    decimal UtilidadEstimada,
    decimal PesoPromedioActualGramos,
    decimal FCRActual,
    IEnumerable<VentaItemResponse> Ventas,
    IEnumerable<MortalidadItemResponse> HistorialMortalidad,
    IEnumerable<GastoItemResponse> Gastos,
    IEnumerable<PesajeItemResponse> Pesajes);

public record VentaItemResponse(Guid Id, DateTime Fecha, int Cantidad, decimal PesoTotalKg, decimal PrecioPorKilo, decimal Total);
public record MortalidadItemResponse(Guid Id, DateTime Fecha, int Cantidad, string Causa);
public record GastoItemResponse(Guid Id, DateTime Fecha, string Descripcion, decimal Monto, string Tipo);
public record PesajeItemResponse(Guid Id, DateTime Fecha, decimal PesoPromedioGramos, int CantidadMuestreada);

public class ObtenerDetalleLoteQueryHandler : IRequestHandler<ObtenerDetalleLoteQuery, LoteDetalleResponse>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IVentaRepository _ventaRepository;
    private readonly IMortalidadRepository _mortalidadRepository;
    private readonly IGastoOperativoRepository _gastoRepository;
    private readonly IPesajeLoteRepository _pesajeRepository;
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;

    public ObtenerDetalleLoteQueryHandler(
        ILoteRepository loteRepository,
        IVentaRepository ventaRepository,
        IMortalidadRepository mortalidadRepository,
        IGastoOperativoRepository gastoRepository,
        IPesajeLoteRepository pesajeRepository,
        IInventarioRepository inventarioRepository,
        IProductoRepository productoRepository)
    {
        _loteRepository = loteRepository;
        _ventaRepository = ventaRepository;
        _mortalidadRepository = mortalidadRepository;
        _gastoRepository = gastoRepository;
        _pesajeRepository = pesajeRepository;
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
    }

    public async Task<LoteDetalleResponse> Handle(ObtenerDetalleLoteQuery request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null) return null!;

        var ventas = await _ventaRepository.ObtenerPorLoteAsync(request.LoteId);
        var mortalidad = await _mortalidadRepository.ObtenerPorLoteAsync(request.LoteId);
        var gastos = await _gastoRepository.ObtenerPorLoteAsync(request.LoteId);
        var pesajes = await _pesajeRepository.ObtenerPorLoteIdAsync(request.LoteId);
        var movimientos = await _inventarioRepository.ObtenerPorLoteIdAsync(request.LoteId);
        var productos = await _productoRepository.ObtenerTodosAsync();

        // CÃ¡lculos financieros
        var totalVentas = ventas.Sum(v => v.Total.Monto);
        var totalGastos = gastos.Sum(g => g.Monto.Monto);
        var costoPollitos = lote.CantidadInicial * lote.CostoUnitarioPollito.Monto;
        var utilidad = totalVentas - totalGastos - costoPollitos;

        // CÃ¡lculos FCR y Peso
        var ultimoPesaje = pesajes.OrderByDescending(p => p.Fecha).FirstOrDefault();
        decimal pesoPromedioActual = ultimoPesaje?.PesoPromedioGramos ?? 0;

        // FCR = Alimento Consumido (Kg) / Incremento de Peso Total (Kg)
        // Incremento de Peso Total = (Peso Actual Kg - Peso Inicial Kg) * Pollos Vivos
        // Peso inicial estimado: 40g (0.04 Kg)
        
        var productosAlimento = productos
            .Where(p => p.Categoria?.Nombre.Equals("Alimento", StringComparison.OrdinalIgnoreCase) == true)
            .ToDictionary(p => p.Id, p => p.PesoUnitarioKg);

        var alimentoConsumidoKg = movimientos
            .Where(m => productosAlimento.ContainsKey(m.ProductoId) && m.Tipo == TipoMovimiento.Salida)
            .Sum(m => m.Cantidad * productosAlimento[m.ProductoId]);

        decimal fcr = 0;
        if (pesoPromedioActual > 40 && alimentoConsumidoKg > 0)
        {
            decimal pesoGanadoKg = (pesoPromedioActual - 40) / 1000;
            decimal biomasaGanadaKg = pesoGanadoKg * lote.CantidadActual;
            if (biomasaGanadaKg > 0)
            {
                fcr = alimentoConsumidoKg / biomasaGanadaKg;
            }
        }

        return new LoteDetalleResponse(
            lote.Id,
            lote.Nombre,
            lote.Nombre, // NombreLote
            lote.GalponId,
            lote.Galpon?.Nombre ?? "N/A", // NombreGalpon
            lote.Galpon?.Nombre ?? "N/A", // GalponNombre
            lote.FechaIngreso,
            lote.FechaIngreso,
            lote.CantidadInicial,
            lote.CantidadActual, // AvesVivas
            lote.CantidadActual,
            lote.MortalidadAcumulada, // MortalidadTotal
            lote.MortalidadAcumulada,
            lote.CantidadInicial > 0 ? Math.Round((decimal)lote.MortalidadAcumulada / lote.CantidadInicial * 100, 1) : 0,
            lote.PollosVendidos,
            lote.CostoUnitarioPollito.Monto,
            lote.EdadSemanas,
            lote.Estado.ToString(),
            totalVentas,
            totalGastos,
            totalGastos + costoPollitos,
            utilidad,
            pesoPromedioActual,
            Math.Round(fcr, 2),
            ventas.Select(v => new VentaItemResponse(v.Id, v.Fecha, v.CantidadPollos, v.PesoTotalVendido, v.PrecioPorKilo.Monto, v.Total.Monto)),
            mortalidad.Select(m => new MortalidadItemResponse(m.Id, m.Fecha, m.CantidadBajas, m.Causa)),
            gastos.Select(g => new GastoItemResponse(g.Id, g.Fecha, g.Descripcion, g.Monto.Monto, g.TipoGasto)),
            pesajes.Select(p => new PesajeItemResponse(p.Id, p.Fecha, p.PesoPromedioGramos, p.CantidadMuestreada))
        );
    }
}
