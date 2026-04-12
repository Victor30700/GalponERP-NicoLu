using GalponERP.Domain.ValueObjects;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;
using GalponERP.Application.Inventario.Queries.ObtenerComprasInventario;

namespace GalponERP.Application.Proveedores.Queries.ObtenerHistorialProveedor;

public record ObtenerHistorialProveedorQuery(Guid ProveedorId) : IRequest<IEnumerable<CompraInventarioResponse>>;

public class ObtenerHistorialProveedorQueryHandler : IRequestHandler<ObtenerHistorialProveedorQuery, IEnumerable<CompraInventarioResponse>>
{
    private readonly ICompraInventarioRepository _compraRepository;

    public ObtenerHistorialProveedorQueryHandler(ICompraInventarioRepository compraRepository)
    {
        _compraRepository = compraRepository;
    }

    public async Task<IEnumerable<CompraInventarioResponse>> Handle(ObtenerHistorialProveedorQuery request, CancellationToken cancellationToken)
    {
        var result = await _compraRepository.ObtenerHistorialProveedorAsync(request.ProveedorId);

        return result.Select(x => new CompraInventarioResponse(
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
