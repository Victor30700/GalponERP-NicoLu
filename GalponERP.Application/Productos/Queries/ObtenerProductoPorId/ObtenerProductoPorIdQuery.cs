using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Productos.Queries.ObtenerProductoPorId;

public record ObtenerProductoPorIdQuery(Guid Id) : IRequest<ProductoResponse?>;

public record ProductoResponse(
    Guid Id,
    string Nombre,
    Guid CategoriaProductoId,
    string CategoriaNombre,
    Guid UnidadMedidaId,
    string UnidadMedidaNombre,
    decimal EquivalenciaEnKg,
    bool IsActive);

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
            p.IsActive);
    }
}
