using MediatR;

namespace GalponERP.Application.Inventario.Queries.ObtenerMovimientosLote;

public record ObtenerMovimientosLoteQuery(Guid LoteId) : IRequest<IEnumerable<MovimientoLoteResponse>>;

public record MovimientoLoteResponse(
    Guid Id,
    Guid ProductoId,
    string NombreProducto,
    string CategoriaProducto,
    decimal Cantidad,
    string Tipo,
    DateTime Fecha);

public class ObtenerMovimientosLoteHandler : IRequestHandler<ObtenerMovimientosLoteQuery, IEnumerable<MovimientoLoteResponse>>
{
    private readonly GalponERP.Domain.Interfaces.Repositories.IInventarioRepository _inventarioRepository;
    private readonly GalponERP.Domain.Interfaces.Repositories.IProductoRepository _productoRepository;
    private readonly GalponERP.Domain.Interfaces.Repositories.ICategoriaProductoRepository _categoriaRepository;

    public ObtenerMovimientosLoteHandler(
        GalponERP.Domain.Interfaces.Repositories.IInventarioRepository inventarioRepository,
        GalponERP.Domain.Interfaces.Repositories.IProductoRepository productoRepository,
        GalponERP.Domain.Interfaces.Repositories.ICategoriaProductoRepository categoriaRepository)
    {
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<IEnumerable<MovimientoLoteResponse>> Handle(ObtenerMovimientosLoteQuery request, CancellationToken cancellationToken)
    {
        var todosLosMovimientos = await _inventarioRepository.ObtenerTodosAsync();
        var movimientosLote = todosLosMovimientos.Where(m => m.LoteId == request.LoteId);

        var productos = (await _productoRepository.ObtenerTodosAsync()).ToDictionary(p => p.Id);
        var categorias = (await _categoriaRepository.ObtenerTodasAsync()).ToDictionary(c => c.Id);

        return movimientosLote.Select(m => {
            var prod = productos.ContainsKey(m.ProductoId) ? productos[m.ProductoId] : null;
            var cat = prod != null && categorias.ContainsKey(prod.CategoriaProductoId) ? categorias[prod.CategoriaProductoId] : null;

            return new MovimientoLoteResponse(
                m.Id,
                m.ProductoId,
                prod?.Nombre ?? "N/A",
                cat?.Nombre ?? "N/A",
                m.Cantidad,
                m.Tipo.ToString(),
                m.Fecha);
        });
    }
}
