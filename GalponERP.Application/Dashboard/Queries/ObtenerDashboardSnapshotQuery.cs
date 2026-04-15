using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Dashboard.Queries;

public record LoteSnapshotDto(string LoteNombre, string GalponNombre, int CantidadActual, int EdadSemanas);
public record AlertaStockDto(string ProductoNombre, decimal StockActual, decimal UmbralMinimo, string Unidad);
public record AlertaSeguridadDto(string UsuarioNombre, string Accion, string EntidadNombre, DateTime Fecha, string Detalles);

public record DashboardSnapshotResponse(
    // ProducciÃ³n
    List<LoteSnapshotDto> LotesActivos,
    int TotalPollosVivos,
    int MortalidadMesActual,
    
    // Inventario
    decimal StockAlimentoKg,
    decimal DiasAlimentoRestantes,
    bool RequiereAlertaAlimento,
    List<AlertaStockDto> AlertasStockMinimo,

    // Finanzas
    decimal SaldoPorCobrarTotal,
    decimal SaldoPorPagarTotal,
    decimal InversionTotalEnCurso,

    // Sanidad
    int TareasSanitariasHoy,

    // Seguridad
    List<AlertaSeguridadDto> AlertasSeguridad24h);

public record ObtenerDashboardSnapshotQuery() : IRequest<DashboardSnapshotResponse>;

public class ObtenerDashboardSnapshotQueryHandler : IRequestHandler<ObtenerDashboardSnapshotQuery, DashboardSnapshotResponse>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IMortalidadRepository _mortalidadRepository;
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IVentaRepository _ventaRepository;
    private readonly ICompraInventarioRepository _compraRepository;
    private readonly ICalendarioSanitarioRepository _calendarioRepository;
    private readonly IGastoOperativoRepository _gastoRepository;
    private readonly IAuditoriaRepository _auditoriaRepository;

    public ObtenerDashboardSnapshotQueryHandler(
        ILoteRepository loteRepository, 
        IMortalidadRepository mortalidadRepository,
        IInventarioRepository inventarioRepository,
        IProductoRepository productoRepository,
        IVentaRepository ventaRepository,
        ICompraInventarioRepository compraRepository,
        ICalendarioSanitarioRepository calendarioRepository,
        IGastoOperativoRepository gastoRepository,
        IAuditoriaRepository auditoriaRepository)
    {
        _loteRepository = loteRepository;
        _mortalidadRepository = mortalidadRepository;
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _ventaRepository = ventaRepository;
        _compraRepository = compraRepository;
        _calendarioRepository = calendarioRepository;
        _gastoRepository = gastoRepository;
        _auditoriaRepository = auditoriaRepository;
    }

    public async Task<DashboardSnapshotResponse> Handle(ObtenerDashboardSnapshotQuery request, CancellationToken cancellationToken)
    {
        // 1. PRODUCCIÃ“N (Lotes Activos y Mortalidad)
        var lotesActivos = (await _loteRepository.ObtenerActivosAsync()).ToList();
        int totalVivos = lotesActivos.Sum(l => l.CantidadActual);

        var inicioMes = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var finMes = inicioMes.AddMonths(1).AddSeconds(-1);
        var mortalidadMes = await _mortalidadRepository.ObtenerPorRangoFechasAsync(inicioMes, finMes);
        int totalMortalidadMes = mortalidadMes.Sum(m => m.CantidadBajas);

        var lotesDto = lotesActivos.Select(l => new LoteSnapshotDto(
            $"Lote-{l.Id.ToString().Substring(0, 5)}", 
            l.Galpon?.Nombre ?? "Desconocido", 
            l.CantidadActual, 
            l.EdadSemanas)).ToList();

        // 2. INVENTARIO (Stock CrÃ­tico y Alimento)
        var productos = (await _productoRepository.ObtenerTodosAsync()).ToList();
        var todosLosMovimientos = (await _inventarioRepository.ObtenerTodosAsync()).ToList();
        
        var alertasStock = new List<AlertaStockDto>();
        decimal stockTotalAlimentoKg = 0;
        decimal consumoDiarioGlobalKg = 0;

        // Calcular Precios Promedios para InversiÃ³n
        var preciosPromedios = todosLosMovimientos
            .Where(m => (m.Tipo == TipoMovimiento.Entrada || m.Tipo == TipoMovimiento.Compra) && m.CostoTotal != null)
            .GroupBy(m => m.ProductoId)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(m => m.CostoTotal!.Monto) / g.Sum(m => m.Cantidad)
            );

        foreach (var p in productos)
        {
            var movimientosP = todosLosMovimientos.Where(m => m.ProductoId == p.Id).ToList();
            var stockActual = movimientosP.Sum(m => (m.Tipo == TipoMovimiento.Entrada || m.Tipo == TipoMovimiento.Compra || m.Tipo == TipoMovimiento.AjusteEntrada) ? m.Cantidad : -m.Cantidad);
            
            if (p.UmbralMinimo > 0 && stockActual < p.UmbralMinimo)
            {
                alertasStock.Add(new AlertaStockDto(p.Nombre, stockActual, p.UmbralMinimo, p.Unidad?.Abreviatura ?? "und"));
            }

            if (p.Categoria?.Nombre.Equals("Alimento", StringComparison.OrdinalIgnoreCase) == true)
            {
                stockTotalAlimentoKg += stockActual * p.PesoUnitarioKg;
                
                foreach (var lote in lotesActivos)
                {
                    var movimientosLote = movimientosP.Where(m => m.LoteId == lote.Id && m.Tipo == TipoMovimiento.Salida).ToList();
                    if (!movimientosLote.Any()) continue;

                    var diasDeVida = (DateTime.UtcNow - lote.FechaIngreso).TotalDays;
                    if (diasDeVida < 1) diasDeVida = 1;

                    var totalConsumidoLoteKg = movimientosLote.Sum(m => m.Cantidad * p.PesoUnitarioKg);
                    consumoDiarioGlobalKg += totalConsumidoLoteKg / (decimal)diasDeVida;
                }
            }
        }

        decimal diasRestantesAlimento = consumoDiarioGlobalKg > 0 ? stockTotalAlimentoKg / consumoDiarioGlobalKg : 999;
        bool requiereAlertaAlimento = diasRestantesAlimento < 3;

        // 3. FINANZAS (Cuentas por Cobrar, Pagar e InversiÃ³n)
        var ventas = await _ventaRepository.ObtenerTodasAsync();
        decimal saldoTotalCobrar = ventas
            .Where(v => v.EstadoPago != EstadoPago.Pagado)
            .Sum(v => v.SaldoPendiente.Monto);

        var compras = await _compraRepository.ObtenerTodasAsync();
        decimal saldoTotalPagar = compras
            .Where(c => c.EstadoPago != EstadoPago.Pagado)
            .Sum(c => c.SaldoPendiente.Monto);

        decimal inversionTotal = 0;
        foreach (var lote in lotesActivos)
        {
            inversionTotal += lote.CantidadInicial * lote.CostoUnitarioPollito.Monto;
            var gastosLote = await _gastoRepository.ObtenerPorLoteAsync(lote.Id);
            inversionTotal += gastosLote.Sum(g => g.Monto.Monto);
            var consumosLote = todosLosMovimientos.Where(m => m.LoteId == lote.Id && (m.Tipo == TipoMovimiento.Salida || m.Tipo == TipoMovimiento.AjusteSalida)).ToList();
            foreach (var c in consumosLote)
            {
                if (preciosPromedios.TryGetValue(c.ProductoId, out decimal precio))
                {
                    inversionTotal += c.Cantidad * precio;
                }
            }
        }

        // 4. SANIDAD (Tareas Hoy)
        int tareasHoy = 0;
        foreach (var lote in lotesActivos)
        {
            var calendario = await _calendarioRepository.ObtenerPorLoteIdAsync(lote.Id);
            var diaActualLote = (int)(DateTime.UtcNow.Date - lote.FechaIngreso.Date).TotalDays + 1;
            tareasHoy += calendario.Count(c => c.DiaDeAplicacion == diaActualLote && c.Estado == EstadoCalendario.Pendiente);
        }

        // 5. SEGURIDAD (Alertas 24h)
        var hoy = DateTime.UtcNow;
        var ayer = hoy.AddDays(-1);
        var logs = await _auditoriaRepository.ObtenerFiltradosAsync(ayer, hoy, null, null);
        
        var alertasSeguridad = logs
            .Where(l => 
                l.Accion.Contains("Eliminar") || 
                l.Accion.Contains("CambiarRol") || 
                l.Accion.Contains("Bloqueo") || 
                l.Detalles.Contains("Inconsistencia"))
            .Select(l => new AlertaSeguridadDto(
                l.UsuarioNombre, 
                l.Accion, 
                l.EntidadNombre, 
                l.Fecha, 
                l.Detalles))
            .ToList();

        return new DashboardSnapshotResponse(
            lotesDto,
            totalVivos,
            totalMortalidadMes,
            Math.Round(stockTotalAlimentoKg, 2),
            Math.Round(diasRestantesAlimento, 1),
            requiereAlertaAlimento,
            alertasStock,
            Math.Round(saldoTotalCobrar, 2),
            Math.Round(saldoTotalPagar, 2),
            Math.Round(inversionTotal, 2),
            tareasHoy,
            alertasSeguridad);
    }
}
