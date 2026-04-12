using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Ventas.Queries.ObtenerPagosPorVenta;

public class ObtenerPagosPorVentaQueryHandler : IRequestHandler<ObtenerPagosPorVentaQuery, IEnumerable<PagoResponse>>
{
    private readonly IVentaRepository _ventaRepository;

    public ObtenerPagosPorVentaQueryHandler(IVentaRepository ventaRepository)
    {
        _ventaRepository = ventaRepository;
    }

    public async Task<IEnumerable<PagoResponse>> Handle(ObtenerPagosPorVentaQuery request, CancellationToken cancellationToken)
    {
        var venta = await _ventaRepository.ObtenerPorIdAsync(request.VentaId);
        
        if (venta == null)
            return Enumerable.Empty<PagoResponse>();

        // Mostramos todos los pagos, incluyendo los inactivos (anulados) para transparencia de auditoría
        return venta.Pagos.Select(p => new PagoResponse(
            p.Id,
            p.VentaId,
            p.Monto.Monto,
            p.FechaPago,
            p.MetodoPago.ToString(),
            p.UsuarioId,
            p.IsActive
        ));
    }
}
