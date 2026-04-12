using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Inventario.Queries.ObtenerComprasInventario;

public class ObtenerComprasInventarioQueryHandler : IRequestHandler<ObtenerComprasInventarioQuery, IEnumerable<CompraInventarioResponse>>
{
    private readonly ICompraInventarioRepository _compraRepository;

    public ObtenerComprasInventarioQueryHandler(ICompraInventarioRepository compraRepository)
    {
        _compraRepository = compraRepository;
    }

    public async Task<IEnumerable<CompraInventarioResponse>> Handle(ObtenerComprasInventarioQuery request, CancellationToken cancellationToken)
    {
        var compras = await _compraRepository.ObtenerTodasConProveedorAsync();

        return compras.Select(x => new CompraInventarioResponse(
            x.Compra.Id,
            x.Compra.ProveedorId,
            x.ProveedorNombre,
            x.Compra.Fecha,
            x.Compra.Total.Monto,
            x.Compra.TotalPagado.Monto,
            x.Compra.SaldoPendiente.Monto,
            x.Compra.EstadoPago,
            x.Compra.Nota));
    }
}
