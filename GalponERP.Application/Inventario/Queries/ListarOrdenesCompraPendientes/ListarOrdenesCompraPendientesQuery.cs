using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Inventario.Queries.ListarOrdenesCompraPendientes;

public record OrdenCompraResponse(
    Guid Id,
    string RazonSocialProveedor,
    DateTime Fecha,
    decimal Total,
    string Estado,
    string? Nota);

public record ListarOrdenesCompraPendientesQuery() : IRequest<IEnumerable<OrdenCompraResponse>>;

public class ListarOrdenesCompraPendientesHandler : IRequestHandler<ListarOrdenesCompraPendientesQuery, IEnumerable<OrdenCompraResponse>>
{
    private readonly IOrdenCompraRepository _ordenCompraRepository;

    public ListarOrdenesCompraPendientesHandler(IOrdenCompraRepository ordenCompraRepository)
    {
        _ordenCompraRepository = ordenCompraRepository;
    }

    public async Task<IEnumerable<OrdenCompraResponse>> Handle(ListarOrdenesCompraPendientesQuery request, CancellationToken cancellationToken)
    {
        var ordenes = await _ordenCompraRepository.ObtenerPendientesAsync();
        return ordenes.Select(o => new OrdenCompraResponse(
            o.Id,
            o.Proveedor?.RazonSocial ?? "N/A",
            o.Fecha,
            o.Total.Monto,
            o.Estado.ToString(),
            o.Nota));
    }
}
