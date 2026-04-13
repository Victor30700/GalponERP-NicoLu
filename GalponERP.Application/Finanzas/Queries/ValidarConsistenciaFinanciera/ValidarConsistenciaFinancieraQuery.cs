using GalponERP.Domain.Interfaces.Repositories;
using MediatR;
using System.Text;

namespace GalponERP.Application.Finanzas.Queries.ValidarConsistenciaFinanciera;

public record ValidarConsistenciaFinancieraQuery() : IRequest<ConsistenciaResponse>;

public record ConsistenciaResponse(
    bool EsConsistente,
    List<string> Inconsistencias,
    decimal TotalCompras,
    decimal TotalPagos,
    decimal TotalVentas,
    decimal TotalCobros);

public class ValidarConsistenciaFinancieraQueryHandler : IRequestHandler<ValidarConsistenciaFinancieraQuery, ConsistenciaResponse>
{
    private readonly ICompraInventarioRepository _compraRepository;
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IVentaRepository _ventaRepository;

    public ValidarConsistenciaFinancieraQueryHandler(
        ICompraInventarioRepository compraRepository,
        IInventarioRepository inventarioRepository,
        IVentaRepository ventaRepository)
    {
        _compraRepository = compraRepository;
        _inventarioRepository = inventarioRepository;
        _ventaRepository = ventaRepository;
    }

    public async Task<ConsistenciaResponse> Handle(ValidarConsistenciaFinancieraQuery request, CancellationToken cancellationToken)
    {
        var inconsistencias = new List<string>();
        
        // 1. AUDITORÍA DE COMPRAS Y ABASTECIMIENTO
        var compras = await _compraRepository.ObtenerTodasAsync();
        var movimientos = await _inventarioRepository.ObtenerTodosAsync();

        decimal totalCompras = compras.Sum(c => c.Total.Monto);
        decimal totalPagos = compras.Sum(c => c.TotalPagado.Monto);

        foreach (var compra in compras)
        {
            var movsAsociados = movimientos.Where(m => m.CompraId == compra.Id).ToList();
            if (!movsAsociados.Any())
            {
                inconsistencias.Add($"Compra {compra.Id.ToString().Substring(0,8)} ({compra.Fecha:dd/MM}): No tiene movimientos de inventario asociados.");
            }
            else
            {
                var totalMov = movsAsociados.Sum(m => m.CostoTotal?.Monto ?? 0);
                if (Math.Abs(totalMov - compra.Total.Monto) > 0.01m)
                {
                    inconsistencias.Add($"Compra {compra.Id.ToString().Substring(0,8)}: Total OC (S/ {compra.Total.Monto}) != Total Inventario (S/ {totalMov}).");
                }
            }

            if (compra.TotalPagado.Monto > compra.Total.Monto + 0.01m)
            {
                inconsistencias.Add($"Compra {compra.Id.ToString().Substring(0,8)}: Pagos (S/ {compra.TotalPagado.Monto}) exceden el total (S/ {compra.Total.Monto}).");
            }
        }

        // 2. AUDITORÍA DE VENTAS Y COBROS (Sprint 84)
        var ventas = await _ventaRepository.ObtenerTodasAsync();
        decimal totalVentas = ventas.Sum(v => v.Total.Monto);
        decimal totalCobros = ventas.Sum(v => (v.Total.Monto - v.SaldoPendiente.Monto));

        foreach (var venta in ventas)
        {
            var montoCobrado = venta.Total.Monto - venta.SaldoPendiente.Monto;
            if (montoCobrado < -0.01m)
            {
                inconsistencias.Add($"Venta {venta.Id.ToString().Substring(0,8)}: Saldo pendiente calculado es mayor al total de la venta.");
            }
            
            if (montoCobrado > venta.Total.Monto + 0.01m)
            {
                inconsistencias.Add($"Venta {venta.Id.ToString().Substring(0,8)}: Total cobrado (S/ {montoCobrado}) excede el total de la venta (S/ {venta.Total.Monto}).");
            }
        }

        return new ConsistenciaResponse(
            !inconsistencias.Any(),
            inconsistencias,
            totalCompras,
            totalPagos,
            totalVentas,
            totalCobros);
    }
}
