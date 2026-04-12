using MediatR;

namespace GalponERP.Application.Inventario.Queries.ObtenerReporteAjustes;

public record AjusteInventarioResponse(
    Guid Id,
    Guid ProductoId,
    string NombreProducto,
    decimal Cantidad,
    string Tipo,
    DateTime Fecha,
    string? Justificacion,
    Guid? LoteId,
    Guid UsuarioId
);

public record ObtenerReporteAjustesInventarioQuery() : IRequest<IEnumerable<AjusteInventarioResponse>>;
