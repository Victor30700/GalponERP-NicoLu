using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Dashboard.Queries;

public record AlertaStockDto(string ProductoNombre, decimal StockActual, decimal UmbralMinimo);

public record ResumenDashboardResponse(
    int TotalPollosVivos,
    int MortalidadMesActual,
    decimal StockAlimentoActual,
    bool RequiereAlertaAlimento,
    decimal DiasAlimentoRestantes,
    decimal SaldoPorCobrarTotal,
    int TareasSanitariasHoy,
    decimal InversionTotalEnCurso,
    List<AlertaStockDto> AlertasStockMinimo);

public record ObtenerResumenDashboardQuery() : IRequest<ResumenDashboardResponse>;

public class ObtenerResumenDashboardQueryHandler : IRequestHandler<ObtenerResumenDashboardQuery, ResumenDashboardResponse>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IMortalidadRepository _mortalidadRepository;
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IVentaRepository _ventaRepository;
    private readonly ICalendarioSanitarioRepository _calendarioRepository;
    private readonly IGastoOperativoRepository _gastoRepository;

    public ObtenerResumenDashboardQueryHandler(
        ILoteRepository loteRepository, 
        IMortalidadRepository mortalidadRepository,
        IInventarioRepository inventarioRepository,
        IProductoRepository productoRepository,
        IVentaRepository ventaRepository,
        ICalendarioSanitarioRepository calendarioRepository,
        IGastoOperativoRepository gastoRepository)
    {
        _loteRepository = loteRepository;
        _mortalidadRepository = mortalidadRepository;
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _ventaRepository = ventaRepository;
        _calendarioRepository = calendarioRepository;
        _gastoRepository = gastoRepository;
    }

    public async Task<ResumenDashboardResponse> Handle(ObtenerResumenDashboardQuery request, CancellationToken cancellationToken)
    {
        // 1. Lotes Activos
        var lotesActivos = (await _loteRepository.ObtenerActivosAsync()).ToList();
        int totalVivos = lotesActivos.Sum(l => l.CantidadActual);

        // 2. Mortalidad Mes Actual
        var inicioMes = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var finMes = inicioMes.AddMonths(1).AddSeconds(-1);
        var mortalidadMes = await _mortalidadRepository.ObtenerPorRangoFechasAsync(inicioMes, finMes);
        int totalMortalidadMes = mortalidadMes.Sum(m => m.CantidadBajas);

        // 3. Productos y Alertas de Stock
        var productos = (await _productoRepository.ObtenerTodosAsync()).ToList();
        var todosLosMovimientos = (await _inventarioRepository.ObtenerTodosAsync()).ToList();
        
        var alertasStock = new List<AlertaStockDto>();
        decimal stockTotalAlimentoKg = 0;
        decimal consumoDiarioGlobalKg = 0;

        // Calcular Precios Promedios para Inversión
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
                alertasStock.Add(new AlertaStockDto(p.Nombre, stockActual, p.UmbralMinimo));
            }

            // Lógica específica para Alimento (Alertas de Días Restantes)
            if (p.Categoria?.Nombre.Equals("Alimento", StringComparison.OrdinalIgnoreCase) == true)
            {
                stockTotalAlimentoKg += stockActual * p.EquivalenciaEnKg;
                
                foreach (var lote in lotesActivos)
                {
                    var movimientosLote = movimientosP.Where(m => m.LoteId == lote.Id && m.Tipo == TipoMovimiento.Salida).ToList();
                    if (!movimientosLote.Any()) continue;

                    var diasDeVida = (DateTime.UtcNow - lote.FechaIngreso).TotalDays;
                    if (diasDeVida < 1) diasDeVida = 1;

                    var totalConsumidoLoteKg = movimientosLote.Sum(m => m.Cantidad * p.EquivalenciaEnKg);
                    consumoDiarioGlobalKg += totalConsumidoLoteKg / (decimal)diasDeVida;
                }
            }
        }

        decimal diasRestantesAlimento = consumoDiarioGlobalKg > 0 ? stockTotalAlimentoKg / consumoDiarioGlobalKg : 999;
        bool requiereAlertaAlimento = diasRestantesAlimento < 3;

        // 4. Saldo Total por Cobrar
        var ventas = await _ventaRepository.ObtenerTodasAsync();
        decimal saldoTotal = ventas
            .Where(v => v.EstadoPago != EstadoPago.Pagado)
            .Sum(v => v.SaldoPendiente.Monto);

        // 5. Tareas Sanitarias Hoy
        int tareasHoy = 0;
        foreach (var lote in lotesActivos)
        {
            var calendario = await _calendarioRepository.ObtenerPorLoteIdAsync(lote.Id);
            var diaActualLote = (int)(DateTime.UtcNow.Date - lote.FechaIngreso.Date).TotalDays + 1;
            tareasHoy += calendario.Count(c => c.DiaDeAplicacion == diaActualLote && c.Estado == EstadoCalendario.Pendiente);
        }

        // 6. Inversión Total en Curso
        decimal inversionTotal = 0;
        foreach (var lote in lotesActivos)
        {
            // Costo Pollitos
            inversionTotal += lote.CantidadInicial * lote.CostoUnitarioPollito.Monto;

            // Gastos Operativos
            var gastosLote = await _gastoRepository.ObtenerPorLoteAsync(lote.Id);
            inversionTotal += gastosLote.Sum(g => g.Monto.Monto);

            // Consumos de Inventario
            var consumosLote = todosLosMovimientos.Where(m => m.LoteId == lote.Id && (m.Tipo == TipoMovimiento.Salida || m.Tipo == TipoMovimiento.AjusteSalida)).ToList();
            foreach (var c in consumosLote)
            {
                if (preciosPromedios.TryGetValue(c.ProductoId, out decimal precio))
                {
                    inversionTotal += c.Cantidad * precio;
                }
            }
        }

        return new ResumenDashboardResponse(
            totalVivos,
            totalMortalidadMes,
            stockTotalAlimentoKg,
            requiereAlertaAlimento,
            diasRestantesAlimento,
            saldoTotal,
            tareasHoy,
            Math.Round(inversionTotal, 2),
            alertasStock);
    }
}
