using MediatR;

namespace GalponERP.Application.Inventario.Queries.ObtenerReporteMovimientos;

public record ObtenerReporteMovimientosQuery(
    DateTime FechaInicio, 
    DateTime FechaFin, 
    Guid? CategoriaProductoId) : IRequest<IEnumerable<ReporteMovimientoResponse>>;

public record ReporteMovimientoResponse(
    Guid Id,
    Guid ProductoId,
    string NombreProducto,
    Guid? CategoriaProductoId,
    string NombreCategoria,
    Guid? LoteId,
    decimal Cantidad,
    string Tipo,
    DateTime Fecha,
    string? Justificacion);
