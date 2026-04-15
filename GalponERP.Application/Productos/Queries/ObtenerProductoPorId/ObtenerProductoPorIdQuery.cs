using GalponERP.Domain.Interfaces.Repositories;
using MediatR;
using GalponERP.Application.Productos.Queries;

namespace GalponERP.Application.Productos.Queries.ObtenerProductoPorId;

public record ObtenerProductoPorIdQuery(Guid Id) : IRequest<ProductoResponse?>;

public class ObtenerProductoPorIdQueryHandler : IRequestHandler<ObtenerProductoPorIdQuery, ProductoResponse?>
{
    private readonly IProductoRepository _productoRepository;
    private readonly IInventarioRepository _inventarioRepository;

    public ObtenerProductoPorIdQueryHandler(IProductoRepository productoRepository, IInventarioRepository inventarioRepository)
    {
        _productoRepository = productoRepository;
        _inventarioRepository = inventarioRepository;
    }

    public async Task<ProductoResponse?> Handle(ObtenerProductoPorIdQuery request, CancellationToken cancellationToken)
    {
        var p = await _productoRepository.ObtenerPorIdAsync(request.Id);
        
        if (p == null)
            return null;

        var stock = await _inventarioRepository.ObtenerStockPorProductoIdAsync(p.Id);
        
        // Sincronización en caliente para display
        p.SincronizarStockKg(stock);

        return new ProductoResponse(
            p.Id,
            p.Nombre,
            p.CategoriaProductoId,
            p.Categoria?.Nombre ?? "Sin Categoría",
            p.UnidadMedidaId,
            p.Unidad?.Nombre ?? "Sin Unidad",
            p.PesoUnitarioKg,
            p.UmbralMinimo,
            stock,
            p.StockActualKg,
            p.IsActive);
    }
}
