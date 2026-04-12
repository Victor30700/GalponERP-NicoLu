using GalponERP.Application.Ventas.Queries.ObtenerVentas;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Ventas.Queries.ObtenerVentaPorId;

public class ObtenerVentaPorIdQueryHandler : IRequestHandler<ObtenerVentaPorIdQuery, VentaResponse?>
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IClienteRepository _clienteRepository;

    public ObtenerVentaPorIdQueryHandler(IVentaRepository ventaRepository, IClienteRepository clienteRepository)
    {
        _ventaRepository = ventaRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<VentaResponse?> Handle(ObtenerVentaPorIdQuery request, CancellationToken cancellationToken)
    {
        var venta = await _ventaRepository.ObtenerPorIdAsync(request.Id);
        
        if (venta == null)
            return null;

        var cliente = await _clienteRepository.ObtenerPorIdAsync(venta.ClienteId);

        return new VentaResponse(
            venta.Id,
            venta.LoteId,
            venta.ClienteId,
            cliente?.Nombre ?? "Cliente desconocido",
            venta.Fecha,
            venta.CantidadPollos,
            venta.PesoTotalVendido,
            venta.PrecioPorKilo.Monto,
            venta.Total.Monto,
            venta.SaldoPendiente.Monto,
            venta.EstadoPago.ToString());
    }
}
