using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.Entities;
using MediatR;

namespace GalponERP.Application.Inventario.Queries.PredecirAgotamientoStock;

public record PredecirAgotamientoStockQuery(Guid ProductoId) : IRequest<PrediccionAgotamientoResponse>;

public record PrediccionAgotamientoResponse(
    Guid ProductoId,
    string NombreProducto,
    decimal StockActual,
    decimal ConsumoPromedioDiario,
    decimal DiasRestantes,
    DateTime FechaEstimadaAgotamiento,
    bool RequierePedidoUrgente);

public class PredecirAgotamientoStockQueryHandler : IRequestHandler<PredecirAgotamientoStockQuery, PrediccionAgotamientoResponse>
{
    private readonly IInventarioRepository _inventarioRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IProductoRepository _productoRepository;

    public PredecirAgotamientoStockQueryHandler(
        IInventarioRepository inventarioRepository, 
        ILoteRepository loteRepository,
        IProductoRepository productoRepository)
    {
        _inventarioRepository = inventarioRepository;
        _loteRepository = loteRepository;
        _productoRepository = productoRepository;
    }

    public async Task<PrediccionAgotamientoResponse> Handle(PredecirAgotamientoStockQuery request, CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.ObtenerPorIdAsync(request.ProductoId);
        if (producto == null) throw new Exception("Producto no encontrado.");

        var stockActual = await _inventarioRepository.ObtenerStockPorProductoIdAsync(request.ProductoId);
        
        // 1. Obtener movimientos de salida de los últimos 14 días para una mejor media
        var movimientos = await _inventarioRepository.ObtenerPorProductoIdAsync(request.ProductoId);
        var salidasRecientes = movimientos
            .Where(m => m.Tipo == TipoMovimiento.Salida || m.Tipo == TipoMovimiento.AjusteSalida)
            .Where(m => m.Fecha >= DateTime.Today.AddDays(-14))
            .ToList();

        decimal consumoPromedioDiario = 0;
        if (salidasRecientes.Any())
        {
            var totalConsumido = salidasRecientes.Sum(m => m.Cantidad);
            consumoPromedioDiario = totalConsumido / 14m;
        }

        // 2. Si no hay salidas recientes, no podemos predecir
        if (consumoPromedioDiario <= 0)
        {
            return new PrediccionAgotamientoResponse(
                producto.Id, producto.Nombre, stockActual, 0, 999, DateTime.MaxValue, false);
        }

        // 3. Calcular días restantes
        decimal diasRestantes = stockActual / consumoPromedioDiario;
        DateTime fechaEstimada = DateTime.Today.AddDays((double)diasRestantes);

        // 4. Umbral de urgencia: menos de 5 días
        bool requierePedido = diasRestantes < 5;

        return new PrediccionAgotamientoResponse(
            producto.Id,
            producto.Nombre,
            stockActual,
            consumoPromedioDiario,
            diasRestantes,
            fechaEstimada,
            requierePedido);
    }
}
