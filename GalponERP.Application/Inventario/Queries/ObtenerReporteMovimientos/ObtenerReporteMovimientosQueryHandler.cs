using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Inventario.Queries.ObtenerReporteMovimientos;

public class ObtenerReporteMovimientosQueryHandler : IRequestHandler<ObtenerReporteMovimientosQuery, IEnumerable<ReporteMovimientoResponse>>
{
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly ICategoriaProductoRepository _categoriaRepository;

    public ObtenerReporteMovimientosQueryHandler(
        IInventarioRepository inventarioRepository, 
        IProductoRepository productoRepository,
        ICategoriaProductoRepository categoriaRepository)
    {
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<IEnumerable<ReporteMovimientoResponse>> Handle(ObtenerReporteMovimientosQuery request, CancellationToken cancellationToken)
    {
        var movimientos = await _inventarioRepository.ObtenerTodosAsync();
        var productos = await _productoRepository.ObtenerTodosAsync();
        var categorias = await _categoriaRepository.ObtenerTodasAsync();

        var productosDict = productos.ToDictionary(p => p.Id, p => p);
        var categoriasDict = categorias.ToDictionary(c => c.Id, c => c.Nombre);

        var query = movimientos
            .Where(m => m.Fecha >= request.FechaInicio && m.Fecha <= request.FechaFin);

        if (request.CategoriaProductoId.HasValue)
        {
            query = query.Where(m => 
                productosDict.TryGetValue(m.ProductoId, out var p) && 
                p.CategoriaProductoId == request.CategoriaProductoId.Value);
        }

        return query
            .OrderByDescending(m => m.Fecha)
            .Select(m => {
                productosDict.TryGetValue(m.ProductoId, out var p);
                var catId = p?.CategoriaProductoId;
                var catNombre = catId.HasValue && categoriasDict.TryGetValue(catId.Value, out var n) ? n : "Desconocida";
                
                return new ReporteMovimientoResponse(
                    m.Id,
                    m.ProductoId,
                    p?.Nombre ?? "Producto desconocido",
                    catId,
                    catNombre,
                    m.LoteId,
                    m.Cantidad,
                    m.Tipo.ToString(),
                    m.Fecha,
                    m.Justificacion);
            });
    }
}
