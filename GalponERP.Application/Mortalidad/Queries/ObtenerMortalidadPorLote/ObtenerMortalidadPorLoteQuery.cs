using MediatR;

namespace GalponERP.Application.Mortalidad.Queries.ObtenerMortalidadPorLote;

public record ObtenerMortalidadPorLoteQuery(Guid LoteId) : IRequest<IEnumerable<MortalidadResponse>>;

public record MortalidadResponse(
    Guid Id,
    Guid LoteId,
    DateTime Fecha,
    int CantidadBajas,
    string Causa);
