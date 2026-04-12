using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Inventario.Queries.ObtenerMovimientos;

public class ObtenerMovimientosQueryHandler : IRequestHandler<ObtenerMovimientosQuery, IEnumerable<MovimientoResponse>>
{
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;

    public ObtenerMovimientosQueryHandler(IInventarioRepository inventarioRepository, IProductoRepository productoRepository)
    {
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
    }

    public async Task<IEnumerable<MovimientoResponse>> Handle(ObtenerMovimientosQuery request, CancellationToken cancellationToken)
    {
        var movimientos = await _inventarioRepository.ObtenerTodosAsync();

        if (request.ProductoId.HasValue)
        {
            movimientos = movimientos.Where(m => m.ProductoId == request.ProductoId.Value);
        }

        var productos = await _productoRepository.ObtenerTodosAsync();
        var productosDict = productos.ToDictionary(p => p.Id, p => p.Nombre);

        return movimientos
            .OrderByDescending(m => m.Fecha)
            .Select(m => new MovimientoResponse(
                m.Id,
                m.ProductoId,
                productosDict.TryGetValue(m.ProductoId, out var nombre) ? nombre : "Producto desconocido",
                m.LoteId,
                m.Cantidad,
                m.Tipo.ToString(),
                m.Fecha,
                m.Justificacion));
    }
}
