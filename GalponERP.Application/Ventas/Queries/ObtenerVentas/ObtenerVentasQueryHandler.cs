using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Ventas.Queries.ObtenerVentas;

public class ObtenerVentasQueryHandler : IRequestHandler<ObtenerVentasQuery, IEnumerable<VentaResponse>>
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IClienteRepository _clienteRepository;

    public ObtenerVentasQueryHandler(IVentaRepository ventaRepository, IClienteRepository clienteRepository)
    {
        _ventaRepository = ventaRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<IEnumerable<VentaResponse>> Handle(ObtenerVentasQuery request, CancellationToken cancellationToken)
    {
        var ventas = await _ventaRepository.ObtenerTodasAsync();
        var clientes = await _clienteRepository.ObtenerTodosAsync();
        var clientesDict = clientes.ToDictionary(c => c.Id, c => c.Nombre);

        return ventas.Select(v => new VentaResponse(
            v.Id,
            v.LoteId,
            v.ClienteId,
            clientesDict.TryGetValue(v.ClienteId, out var nombre) ? nombre : "Cliente desconocido",
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
