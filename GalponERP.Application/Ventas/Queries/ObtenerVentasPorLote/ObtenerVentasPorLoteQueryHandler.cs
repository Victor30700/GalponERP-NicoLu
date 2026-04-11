using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.Ventas.Queries.ObtenerVentas;
using MediatR;

namespace GalponERP.Application.Ventas.Queries.ObtenerVentasPorLote;

public class ObtenerVentasPorLoteQueryHandler : IRequestHandler<ObtenerVentasPorLoteQuery, IEnumerable<VentaResponse>>
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IClienteRepository _clienteRepository;

    public ObtenerVentasPorLoteQueryHandler(IVentaRepository ventaRepository, IClienteRepository clienteRepository)
    {
        _ventaRepository = ventaRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<IEnumerable<VentaResponse>> Handle(ObtenerVentasPorLoteQuery request, CancellationToken cancellationToken)
    {
        var ventas = await _ventaRepository.ObtenerPorLoteAsync(request.LoteId);
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
            v.Total.Monto));
    }
}
