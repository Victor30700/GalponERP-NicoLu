using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Productos.Queries.ListarProductos;

public record ListarProductosQuery() : IRequest<IEnumerable<ProductoResponse>>;

public record ProductoResponse(
    Guid Id,
    string Nombre,
    TipoProducto Tipo,
    UnidadMedida UnidadMedida,
    bool IsActive);

public class ListarProductosQueryHandler : IRequestHandler<ListarProductosQuery, IEnumerable<ProductoResponse>>
{
    private readonly IProductoRepository _productoRepository;

    public ListarProductosQueryHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<IEnumerable<ProductoResponse>> Handle(ListarProductosQuery request, CancellationToken cancellationToken)
    {
        var productos = await _productoRepository.ObtenerTodosAsync();
        
        return productos.Select(p => new ProductoResponse(
            p.Id,
            p.Nombre,
            p.Tipo,
            p.UnidadMedida,
            p.IsActive));
    }
}
