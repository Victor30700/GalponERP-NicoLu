using MediatR;

namespace GalponERP.Application.Finanzas.Queries.ObtenerGastosGlobales;

public record GastoGlobalResponse(
    Guid Id,
    Guid GalponId,
    Guid? LoteId,
    string Descripcion,
    decimal Monto,
    DateTime Fecha,
    string TipoGasto,
    Guid UsuarioId
);

public record ObtenerGastosGlobalesQuery(
    DateTime? FechaInicio = null,
    DateTime? FechaFin = null,
    string? Categoria = null
) : IRequest<IEnumerable<GastoGlobalResponse>>;
