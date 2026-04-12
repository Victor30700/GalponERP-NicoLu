using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Finanzas.Queries.ObtenerFlujoCajaProyectado;

public class ObtenerFlujoCajaProyectadoQueryHandler : IRequestHandler<ObtenerFlujoCajaProyectadoQuery, FlujoCajaProyectadoResponse>
{
    private readonly IVentaRepository _ventaRepository;
    private readonly ICompraInventarioRepository _compraRepository;
    private readonly IGastoOperativoRepository _gastoRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IProductoRepository _productoRepository;

    public ObtenerFlujoCajaProyectadoQueryHandler(
        IVentaRepository ventaRepository,
        ICompraInventarioRepository compraRepository,
        IGastoOperativoRepository gastoRepository,
        ILoteRepository loteRepository,
        IProductoRepository productoRepository)
    {
        _ventaRepository = ventaRepository;
        _compraRepository = compraRepository;
        _gastoRepository = gastoRepository;
        _loteRepository = loteRepository;
        _productoRepository = productoRepository;
    }

    public async Task<FlujoCajaProyectadoResponse> Handle(ObtenerFlujoCajaProyectadoQuery request, CancellationToken cancellationToken)
    {
        // 1. Saldo Actual (Histórico acumulado simple: Ventas Pagadas - Gastos - Compras Pagadas)
        // Nota: En un sistema real esto vendría de una tabla de Cuentas Bancarias/Caja. 
        // Aquí lo derivamos de transacciones para fines del Sprint.
        var todasVentas = await _ventaRepository.ObtenerTodasAsync();
        var todosGastos = await _gastoRepository.ObtenerTodosAsync();
        var todasCompras = await _compraRepository.ObtenerTodasAsync();

        decimal ingresosEfectivos = todasVentas.Sum(v => v.Total.Monto - v.SaldoPendiente.Monto);
        decimal egresosGastos = todosGastos.Sum(g => g.Monto.Monto);
        decimal egresosCompras = todasCompras.Sum(c => c.TotalPagado.Monto);
        decimal saldoActual = ingresosEfectivos - egresosGastos - egresosCompras;

        // 2. Cuentas por Cobrar (Entradas proyectadas)
        var cuentasPorCobrar = todasVentas.Where(v => v.SaldoPendiente.Monto > 0).ToList();
        decimal totalCXC = cuentasPorCobrar.Sum(v => v.SaldoPendiente.Monto);

        // 3. Cuentas por Pagar (Salidas proyectadas)
        var cuentasPorPagar = todasCompras.Where(c => c.SaldoPendiente.Monto > 0).ToList();
        decimal totalCXP = cuentasPorPagar.Sum(c => c.SaldoPendiente.Monto);

        // 4. Proyección de Consumo de Alimento (Próximos 30 días)
        var lotesActivos = (await _loteRepository.ObtenerTodosAsync())
            .Where(l => l.Estado == EstadoLote.Activo)
            .ToList();

        var productos = await _productoRepository.ObtenerTodosAsync();
        var productosAlimento = productos
            .Where(p => p.Categoria?.Nombre.Equals("Alimento", StringComparison.OrdinalIgnoreCase) == true)
            .ToList();

        decimal costoProyectadoAlimento30Dias = 0;
        foreach (var lote in lotesActivos)
        {
            for (int i = 1; i <= 30; i++)
            {
                int edadDias = (DateTime.UtcNow - lote.FechaIngreso).Days + i;
                decimal consumoPorPolloGramos = ObtenerConsumoEstimadoGramos(edadDias);
                decimal consumoLoteKg = (lote.CantidadActual * consumoPorPolloGramos) / 1000m;

                // Valorar este consumo con el PPP de alguno de los productos de alimento (simplificado)
                var primerAlimento = productosAlimento.FirstOrDefault();
                if (primerAlimento != null)
                {
                    costoProyectadoAlimento30Dias += consumoLoteKg * primerAlimento.CostoUnitarioActual;
                }
            }
        }

        // 5. Consolidar Detalle
        var detalle = new List<DetalleProyeccionDto>();
        
        foreach (var v in cuentasPorCobrar)
            detalle.Add(new DetalleProyeccionDto($"CxC: Venta {v.Id.ToString().Substring(0,8)}", v.SaldoPendiente.Monto, "Ingreso", v.Fecha.AddDays(15)));

        foreach (var c in cuentasPorPagar)
            detalle.Add(new DetalleProyeccionDto($"CxP: Compra {c.Id.ToString().Substring(0,8)}", c.SaldoPendiente.Monto, "Egreso", c.Fecha.AddDays(15)));

        detalle.Add(new DetalleProyeccionDto("Proyección Alimento (30 días)", Math.Round(costoProyectadoAlimento30Dias, 2), "Proyeccion", DateTime.UtcNow.AddDays(30)));

        decimal flujoNeto = totalCXC - totalCXP - costoProyectadoAlimento30Dias;

        return new FlujoCajaProyectadoResponse(
            Math.Round(saldoActual, 2),
            Math.Round(totalCXC, 2),
            Math.Round(totalCXP, 2),
            Math.Round(costoProyectadoAlimento30Dias, 2),
            Math.Round(saldoActual + flujoNeto, 2),
            detalle
        );
    }

    private decimal ObtenerConsumoEstimadoGramos(int dia)
    {
        if (dia <= 7) return 20;
        if (dia <= 14) return 50;
        if (dia <= 21) return 90;
        if (dia <= 28) return 130;
        if (dia <= 35) return 170;
        return 200;
    }
}
