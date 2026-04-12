using MediatR;

namespace GalponERP.Application.Inventario.Queries.ObtenerKardexProducto;

public record ObtenerKardexProductoQuery(Guid ProductoId) : IRequest<IEnumerable<KardexMovimientoResponse>>;

public record KardexMovimientoResponse(
    DateTime Fecha,
    string Tipo,
    decimal Cantidad,
    decimal SaldoAcumulado,
    string? Justificacion,
    Guid? LoteId);
