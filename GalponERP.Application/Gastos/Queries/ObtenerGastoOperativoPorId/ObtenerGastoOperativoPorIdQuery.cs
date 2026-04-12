using MediatR;

namespace GalponERP.Application.Gastos.Queries.ObtenerGastoOperativoPorId;

public record ObtenerGastoOperativoPorIdQuery(Guid Id) : IRequest<GastoOperativoResponse?>;

public record GastoOperativoResponse(
    Guid Id,
    Guid GalponId,
    Guid? LoteId,
    string Descripcion,
    decimal Monto,
    DateTime Fecha,
    string TipoGasto,
    Guid UsuarioId);
