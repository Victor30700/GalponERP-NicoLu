using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Inventario.Queries.ListarPagosCompra;

public class ListarPagosCompraQueryHandler : IRequestHandler<ListarPagosCompraQuery, IEnumerable<PagoCompraDto>>
{
    private readonly ICompraInventarioRepository _compraRepository;

    public ListarPagosCompraQueryHandler(ICompraInventarioRepository compraRepository)
    {
        _compraRepository = compraRepository;
    }

    public async Task<IEnumerable<PagoCompraDto>> Handle(ListarPagosCompraQuery request, CancellationToken cancellationToken)
    {
        var compra = await _compraRepository.ObtenerPorIdAsync(request.CompraId);
        if (compra == null)
            return Enumerable.Empty<PagoCompraDto>();

        return compra.Pagos
            .Where(p => p.IsActive)
            .Select(p => new PagoCompraDto(
                p.Id,
                p.CompraId,
                p.Monto.Monto,
                p.FechaPago,
                p.MetodoPago,
                p.UsuarioId))
            .ToList();
    }
}
