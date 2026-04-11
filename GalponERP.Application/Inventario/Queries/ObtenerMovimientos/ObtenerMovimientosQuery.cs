using GalponERP.Domain.Entities;
using MediatR;

namespace GalponERP.Application.Inventario.Queries.ObtenerMovimientos;

public record ObtenerMovimientosQuery() : IRequest<IEnumerable<MovimientoResponse>>;

public record MovimientoResponse(
    Guid Id,
    Guid ProductoId,
    string NombreProducto,
    Guid? LoteId,
    decimal Cantidad,
    string Tipo,
    DateTime Fecha);
