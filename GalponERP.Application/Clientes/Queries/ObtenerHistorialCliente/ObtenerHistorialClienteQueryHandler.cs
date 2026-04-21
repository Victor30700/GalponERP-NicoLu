using GalponERP.Application.Ventas.Queries.ObtenerVentas;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Clientes.Queries.ObtenerHistorialCliente;

public class ObtenerHistorialClienteQueryHandler : IRequestHandler<ObtenerHistorialClienteQuery, IEnumerable<VentaResponse>>
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IClienteRepository _clienteRepository;

    public ObtenerHistorialClienteQueryHandler(IVentaRepository ventaRepository, IClienteRepository clienteRepository)
    {
        _ventaRepository = ventaRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<IEnumerable<VentaResponse>> Handle(ObtenerHistorialClienteQuery request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObtenerPorIdAsync(request.ClienteId);
        if (cliente == null)
            throw new Exception($"Cliente con ID {request.ClienteId} no encontrado.");

        var ventas = await _ventaRepository.ObtenerPorClienteAsync(request.ClienteId);

        return ventas.Select(v => new VentaResponse(
            v.Id,
            v.LoteId,
            v.ClienteId,
            cliente.Nombre,
            v.Fecha,
            v.CantidadPollos,
            v.PesoTotalVendido,
            v.PrecioPorKilo.Monto,
            v.Total.Monto,
            v.SaldoPendiente.Monto,
            v.EstadoPago.ToString(),
            v.Version.ToString()));
    }
}
