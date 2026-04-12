using MediatR;

namespace GalponERP.Application.Mortalidad.Queries.ObtenerMortalidadPorId;

public record ObtenerMortalidadPorIdQuery(Guid Id) : IRequest<MortalidadResponse?>;

public record MortalidadResponse(
    Guid Id,
    Guid LoteId,
    DateTime Fecha,
    int CantidadBajas,
    string Causa,
    Guid UsuarioId);
