using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Dashboard.Queries.ObtenerComparativaLotes;

public class ObtenerComparativaLotesQueryHandler : IRequestHandler<ObtenerComparativaLotesQuery, IEnumerable<LoteComparativoResponse>>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IVentaRepository _ventaRepository;
    private readonly IGastoOperativoRepository _gastoRepository;
    private readonly IPesajeLoteRepository _pesajeRepository;
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;

    public ObtenerComparativaLotesQueryHandler(
        ILoteRepository loteRepository,
        IVentaRepository ventaRepository,
        IGastoOperativoRepository gastoRepository,
        IPesajeLoteRepository pesajeRepository,
        IInventarioRepository inventarioRepository,
        IProductoRepository productoRepository)
    {
        _loteRepository = loteRepository;
        _ventaRepository = ventaRepository;
        _gastoRepository = gastoRepository;
        _pesajeRepository = pesajeRepository;
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
    }

    public async Task<IEnumerable<LoteComparativoResponse>> Handle(ObtenerComparativaLotesQuery request, CancellationToken cancellationToken)
    {
        var lotes = await _loteRepository.ObtenerTodosAsync();
        var productos = await _productoRepository.ObtenerTodosAsync();
        var alimentoIds = productos.Where(p => p.Tipo == TipoProducto.Alimento).Select(p => p.Id).ToList();

        var comparativa = new List<LoteComparativoResponse>();

        foreach (var lote in lotes.OrderByDescending(l => l.FechaIngreso).Take(5)) // Comparar los últimos 5
        {
            var ventas = await _ventaRepository.ObtenerPorLoteAsync(lote.Id);
            var gastos = await _gastoRepository.ObtenerPorLoteAsync(lote.Id);
            var pesajes = await _pesajeRepository.ObtenerPorLoteIdAsync(lote.Id);
            var movimientos = await _inventarioRepository.ObtenerPorLoteIdAsync(lote.Id);

            var totalVentas = ventas.Sum(v => v.Total.Monto);
            var totalGastos = gastos.Sum(g => g.Monto.Monto);
            var costoPollitos = lote.CantidadInicial * lote.CostoUnitarioPollito.Monto;
            var utilidadNeta = totalVentas - totalGastos - costoPollitos;

            var ultimoPesaje = pesajes.OrderByDescending(p => p.Fecha).FirstOrDefault();
            decimal fcr = 0;
            if (ultimoPesaje != null)
            {
                var alimentoConsumidoKg = movimientos
                    .Where(m => alimentoIds.Contains(m.ProductoId) && m.Tipo == TipoMovimiento.Salida)
                    .Sum(m => m.Cantidad);

                decimal biomasaGanadaKg = ((ultimoPesaje.PesoPromedioGramos - 40) / 1000) * lote.CantidadActual;
                if (biomasaGanadaKg > 0)
                {
                    fcr = alimentoConsumidoKg / biomasaGanadaKg;
                }
            }

            comparativa.Add(new LoteComparativoResponse(
                lote.Id,
                lote.FechaIngreso,
                lote.CantidadInicial,
                lote.MortalidadAcumulada,
                Math.Round(fcr, 2),
                totalVentas,
                totalGastos,
                utilidadNeta));
        }

        return comparativa;
    }
}
