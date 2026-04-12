using MediatR;

namespace GalponERP.Application.Finanzas.Queries.ObtenerCuentasPorCobrar;

public record ObtenerCuentasPorCobrarQuery() : IRequest<IEnumerable<CuentaPorCobrarDto>>;

public record CuentaPorCobrarDto(
    Guid VentaId,
    DateTime Fecha,
    string ClienteNombre,
    string LoteCodigo,
    decimal TotalVenta,
    decimal TotalPagado,
    decimal SaldoPendiente,
    string EstadoPago);
