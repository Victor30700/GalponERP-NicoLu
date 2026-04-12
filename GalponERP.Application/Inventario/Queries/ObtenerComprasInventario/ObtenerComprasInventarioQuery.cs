using GalponERP.Domain.ValueObjects;
using GalponERP.Domain.Entities;
using MediatR;

namespace GalponERP.Application.Inventario.Queries.ObtenerComprasInventario;

public record CompraInventarioResponse(
    Guid Id,
    Guid ProveedorId,
    string ProveedorNombre,
    DateTime Fecha,
    decimal Total,
    decimal TotalPagado,
    decimal SaldoPendiente,
    EstadoPago EstadoPago,
    string? Nota);

public record ObtenerComprasInventarioQuery() : IRequest<IEnumerable<CompraInventarioResponse>>;
