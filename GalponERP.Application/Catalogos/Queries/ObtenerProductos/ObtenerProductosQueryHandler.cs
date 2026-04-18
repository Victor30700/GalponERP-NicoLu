using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.Entities;
using MediatR;
using GalponERP.Application.Productos.Queries;

namespace GalponERP.Application.Catalogos.Queries.ObtenerProductos;

public class ObtenerProductosQueryHandler : IRequestHandler<ObtenerProductosQuery, IEnumerable<ProductoResponse>>
{
    private readonly IProductoRepository _productoRepository;
    private readonly IInventarioRepository _inventarioRepository;

    public ObtenerProductosQueryHandler(IProductoRepository productoRepository, IInventarioRepository inventarioRepository)
    {
        _productoRepository = productoRepository;
        _inventarioRepository = inventarioRepository;
    }

    public async Task<IEnumerable<ProductoResponse>> Handle(ObtenerProductosQuery request, CancellationToken cancellationToken)
    {
        var productos = await _productoRepository.ObtenerTodosAsync();
        var movimientos = await _inventarioRepository.ObtenerTodosAsync();

        var stockPorProducto = movimientos
            .GroupBy(m => m.ProductoId)
            .ToDictionary(
                g => g.Key, 
                g => g.Sum(m => m.Tipo == TipoMovimiento.Entrada || m.Tipo == TipoMovimiento.AjusteEntrada || m.Tipo == TipoMovimiento.Compra 
                    ? m.Cantidad 
                    : -m.Cantidad)
            );

        return productos.Select(p => new ProductoResponse(
            p.Id,
            p.Nombre,
            p.CategoriaProductoId,
            p.Categoria?.Nombre ?? "Sin Categoría",
            p.UnidadMedidaId,
            p.Unidad?.Nombre ?? "Sin Unidad",
            p.PesoUnitarioKg,
            p.UmbralMinimo,
            stockPorProducto.ContainsKey(p.Id) ? stockPorProducto[p.Id] : 0,
            p.StockActualKg,
            p.IsActive,
            p.Unidad?.Tipo ?? TipoUnidad.UnidadFisica));
    }
}
