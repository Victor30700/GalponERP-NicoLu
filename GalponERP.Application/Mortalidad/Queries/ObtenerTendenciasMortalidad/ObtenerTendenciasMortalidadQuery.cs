using MediatR;

namespace GalponERP.Application.Mortalidad.Queries.ObtenerTendenciasMortalidad;

public record ObtenerTendenciasMortalidadQuery(Guid LoteId) : IRequest<TendenciasMortalidadResponse>;

public record TendenciasMortalidadResponse(
    Guid LoteId,
    IEnumerable<MortalidadPorSemanaDto> Tendencias);

public record MortalidadPorSemanaDto(
    int SemanaVida,
    int CantidadBajas,
    decimal PorcentajeSemanal);
