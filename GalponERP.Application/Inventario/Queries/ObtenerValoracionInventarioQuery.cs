using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Inventario.Queries.ObtenerValoracionInventario;

public record ObtenerValoracionInventarioQuery() : IRequest<ValoracionInventarioResponse>;

public record ValoracionInventarioResponse(
    decimal ValorTotalEmpresa,
    IEnumerable<DetalleValoracionProducto> Detalles);

public record DetalleValoracionProducto(
    Guid ProductoId,
    string NombreProducto,
    decimal StockActual,
    decimal CostoUnitarioPPP,
    decimal ValorTotal);

public class ObtenerValoracionInventarioQueryHandler : IRequestHandler<ObtenerValoracionInventarioQuery, ValoracionInventarioResponse>
{
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;

    public ObtenerValoracionInventarioQueryHandler(IInventarioRepository inventarioRepository, IProductoRepository productoRepository)
    {
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
    }

    public async Task<ValoracionInventarioResponse> Handle(ObtenerValoracionInventarioQuery request, CancellationToken cancellationToken)
    {
        var productos = await _productoRepository.ObtenerTodosAsync();
        var detalles = new List<DetalleValoracionProducto>();
        decimal valorTotalEmpresa = 0;

        foreach (var producto in productos)
        {
            decimal stock = await _inventarioRepository.ObtenerStockPorProductoIdAsync(producto.Id);
            if (stock == 0 && producto.CostoUnitarioActual == 0) continue;

            decimal valorTotal = stock * producto.CostoUnitarioActual;
            
            detalles.Add(new DetalleValoracionProducto(
                producto.Id,
                producto.Nombre,
                stock,
                producto.CostoUnitarioActual,
                valorTotal));

            valorTotalEmpresa += valorTotal;
        }

        return new ValoracionInventarioResponse(valorTotalEmpresa, detalles);
    }
}
