using GalponERP.Domain.Interfaces.Repositories;
using MediatR;
using GalponERP.Application.Productos.Queries;

namespace GalponERP.Application.Productos.Queries.ObtenerProductoPorId;

public record ObtenerProductoPorIdQuery(Guid Id) : IRequest<ProductoResponse?>;

public class ObtenerProductoPorIdQueryHandler : IRequestHandler<ObtenerProductoPorIdQuery, ProductoResponse?>
{
    private readonly IProductoRepository _productoRepository;

    public ObtenerProductoPorIdQueryHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<ProductoResponse?> Handle(ObtenerProductoPorIdQuery request, CancellationToken cancellationToken)
    {
        var p = await _productoRepository.ObtenerPorIdAsync(request.Id);
        
        if (p == null)
            return null;

        return new ProductoResponse(
            p.Id,
            p.Nombre,
            p.CategoriaProductoId,
            p.Categoria?.Nombre ?? "Sin Categoría",
            p.UnidadMedidaId,
            p.Unidad?.Nombre ?? "Sin Unidad",
            p.EquivalenciaEnKg,
            p.UmbralMinimo,
            p.IsActive);
    }
}
