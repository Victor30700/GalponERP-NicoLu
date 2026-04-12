using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Dashboard.Queries;

public record ResumenDashboardResponse(
    int TotalPollosVivos,
    int MortalidadMesActual,
    decimal StockAlimentoActual,
    bool RequiereAlertaAlimento,
    decimal DiasAlimentoRestantes,
    decimal SaldoPorCobrarTotal,
    int TareasSanitariasHoy);

public record ObtenerResumenDashboardQuery() : IRequest<ResumenDashboardResponse>;

public class ObtenerResumenDashboardQueryHandler : IRequestHandler<ObtenerResumenDashboardQuery, ResumenDashboardResponse>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IMortalidadRepository _mortalidadRepository;
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IVentaRepository _ventaRepository;
    private readonly ICalendarioSanitarioRepository _calendarioRepository;

    public ObtenerResumenDashboardQueryHandler(
        ILoteRepository loteRepository, 
        IMortalidadRepository mortalidadRepository,
        IInventarioRepository inventarioRepository,
        IProductoRepository productoRepository,
        IVentaRepository ventaRepository,
        ICalendarioSanitarioRepository calendarioRepository)
    {
        _loteRepository = loteRepository;
        _mortalidadRepository = mortalidadRepository;
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _ventaRepository = ventaRepository;
        _calendarioRepository = calendarioRepository;
    }

    public async Task<ResumenDashboardResponse> Handle(ObtenerResumenDashboardQuery request, CancellationToken cancellationToken)
    {
        // 1. Total Pollos Vivos (Suma de CantidadActual de lotes activos)
        var lotesActivos = await _loteRepository.ObtenerActivosAsync();
        int totalVivos = lotesActivos.Sum(l => l.CantidadActual);

        // 2. Mortalidad Mes Actual
        var inicioMes = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var finMes = inicioMes.AddMonths(1).AddSeconds(-1);
        var mortalidadMes = await _mortalidadRepository.ObtenerPorRangoFechasAsync(inicioMes, finMes);
        int totalMortalidadMes = mortalidadMes.Sum(m => m.CantidadBajas);

        // 3. Alertas de Alimento
        var productos = await _productoRepository.ObtenerTodosAsync();
        var alimentos = productos.Where(p => p.Categoria?.Nombre == "Alimento");
        var alimentoIds = alimentos.Select(p => p.Id).ToHashSet();
        var todosLosMovimientos = await _inventarioRepository.ObtenerTodosAsync();
        var movimientosAlimento = todosLosMovimientos.Where(m => alimentoIds.Contains(m.ProductoId)).ToList();
        
        var stockTotalAlimento = movimientosAlimento.Sum(m => (m.Tipo == TipoMovimiento.Entrada || m.Tipo == TipoMovimiento.AjusteEntrada) ? m.Cantidad : -m.Cantidad);
        
        decimal consumoDiarioGlobal = 0;
        foreach (var lote in lotesActivos)
        {
            var movimientosLote = movimientosAlimento.Where(m => m.LoteId == lote.Id && (m.Tipo == TipoMovimiento.Salida || m.Tipo == TipoMovimiento.AjusteSalida)).ToList();
            if (!movimientosLote.Any()) continue;

            var diasDeVida = (DateTime.UtcNow - lote.FechaIngreso).TotalDays;
            if (diasDeVida < 1) diasDeVida = 1;

            var totalConsumidoLote = movimientosLote.Sum(m => m.Cantidad);
            var consumoPromedioLote = totalConsumidoLote / (decimal)diasDeVida;
            consumoDiarioGlobal += consumoPromedioLote;
        }

        decimal diasRestantes = consumoDiarioGlobal > 0 ? stockTotalAlimento / consumoDiarioGlobal : 999;
        bool requiereAlerta = diasRestantes < 3;

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

        return new ResumenDashboardResponse(
            totalVivos,
            totalMortalidadMes,
            stockTotalAlimento,
            requiereAlerta,
            diasRestantes,
            saldoTotal,
            tareasHoy);
    }
}
