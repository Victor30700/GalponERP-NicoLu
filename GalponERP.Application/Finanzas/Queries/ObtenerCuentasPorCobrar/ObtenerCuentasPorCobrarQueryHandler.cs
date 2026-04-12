using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.Entities;
using MediatR;

namespace GalponERP.Application.Finanzas.Queries.ObtenerCuentasPorCobrar;

public class ObtenerCuentasPorCobrarQueryHandler : IRequestHandler<ObtenerCuentasPorCobrarQuery, IEnumerable<CuentaPorCobrarDto>>
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly ILoteRepository _loteRepository;

    public ObtenerCuentasPorCobrarQueryHandler(
        IVentaRepository ventaRepository,
        IClienteRepository clienteRepository,
        ILoteRepository loteRepository)
    {
        _ventaRepository = ventaRepository;
        _clienteRepository = clienteRepository;
        _loteRepository = loteRepository;
    }

    public async Task<IEnumerable<CuentaPorCobrarDto>> Handle(ObtenerCuentasPorCobrarQuery request, CancellationToken cancellationToken)
    {
        var ventas = (await _ventaRepository.ObtenerTodasAsync())
            .Where(v => v.EstadoPago != EstadoPago.Pagado);

        var clientes = (await _clienteRepository.ObtenerTodosAsync()).ToDictionary(c => c.Id, c => c.Nombre);
        var lotes = (await _loteRepository.ObtenerTodosAsync()).ToDictionary(l => l.Id, l => l.Id.ToString().Substring(0, 8));

        return ventas.Select(v => new CuentaPorCobrarDto(
            v.Id,
            v.Fecha,
            clientes.TryGetValue(v.ClienteId, out var nombre) ? nombre : "Desconocido",
            lotes.TryGetValue(v.LoteId, out var codigo) ? codigo : "N/A",
            v.Total.Monto,
            v.Total.Monto - v.SaldoPendiente.Monto,
            v.SaldoPendiente.Monto,
            v.EstadoPago.ToString()
        ));
    }
}
