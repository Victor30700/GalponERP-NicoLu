using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Finanzas.Queries.ObtenerCuentasPorPagar;

public record ObtenerCuentasPorPagarQuery() : IRequest<IEnumerable<CuentasPorPagarResponse>>;

public record CuentasPorPagarResponse(
    Guid CompraId,
    Guid ProveedorId,
    string RazonSocialProveedor,
    DateTime Fecha,
    decimal Total,
    decimal TotalPagado,
    decimal SaldoPendiente,
    EstadoPago EstadoPago);

public class ObtenerCuentasPorPagarQueryHandler : IRequestHandler<ObtenerCuentasPorPagarQuery, IEnumerable<CuentasPorPagarResponse>>
{
    private readonly ICompraInventarioRepository _compraRepository;
    private readonly IProveedorRepository _proveedorRepository;

    public ObtenerCuentasPorPagarQueryHandler(ICompraInventarioRepository compraRepository, IProveedorRepository proveedorRepository)
    {
        _compraRepository = compraRepository;
        _proveedorRepository = proveedorRepository;
    }

    public async Task<IEnumerable<CuentasPorPagarResponse>> Handle(ObtenerCuentasPorPagarQuery request, CancellationToken cancellationToken)
    {
        var compras = await _compraRepository.ObtenerTodasAsync();
        var proveedores = await _proveedorRepository.ObtenerTodosAsync();

        return compras
            .Where(c => c.SaldoPendiente.Monto > 0)
            .Select(c => {
                var proveedor = proveedores.FirstOrDefault(p => p.Id == c.ProveedorId);
                return new CuentasPorPagarResponse(
                    c.Id,
                    c.ProveedorId,
                    proveedor?.RazonSocial ?? "Proveedor no encontrado",
                    c.Fecha,
                    c.Total.Monto,
                    c.TotalPagado.Monto,
                    c.SaldoPendiente.Monto,
                    c.EstadoPago);
            });
    }
}
