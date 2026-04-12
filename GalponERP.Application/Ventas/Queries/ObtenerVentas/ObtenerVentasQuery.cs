using MediatR;

namespace GalponERP.Application.Ventas.Queries.ObtenerVentas;

public record ObtenerVentasQuery() : IRequest<IEnumerable<VentaResponse>>;

public record VentaResponse(
    Guid Id,
    Guid LoteId,
    Guid ClienteId,
    string ClienteNombre,
    DateTime Fecha,
    int CantidadPollos,
    decimal PesoTotalKg,
    decimal PrecioPorKilo,
    decimal Total,
    decimal SaldoPendiente,
    string EstadoPago);
