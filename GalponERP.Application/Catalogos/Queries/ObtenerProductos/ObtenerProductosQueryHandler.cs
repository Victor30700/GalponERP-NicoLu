using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Catalogos.Queries.ObtenerProductos;

public class ObtenerProductosQueryHandler : IRequestHandler<ObtenerProductosQuery, IEnumerable<ProductoResponse>>
{
    private readonly IProductoRepository _productoRepository;

    public ObtenerProductosQueryHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<IEnumerable<ProductoResponse>> Handle(ObtenerProductosQuery request, CancellationToken cancellationToken)
    {
        var productos = await _productoRepository.ObtenerTodosAsync();
        
        return productos.Select(p => new ProductoResponse(
            p.Id,
            p.Nombre,
            p.CategoriaProductoId,
            p.Categoria?.Nombre ?? "Sin Categoria",
            p.UnidadMedidaId,
            p.Unidad?.Nombre ?? "Sin Unidad",
            p.EquivalenciaEnKg));
    }
}
