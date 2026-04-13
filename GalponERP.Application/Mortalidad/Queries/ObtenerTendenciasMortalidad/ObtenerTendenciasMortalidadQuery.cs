using MediatR;

namespace GalponERP.Application.Mortalidad.Queries.ObtenerTendenciasMortalidad;

public record ObtenerTendenciasMortalidadQuery(Guid LoteId) : IRequest<IEnumerable<TendenciaChartDto>>;

public record TendenciaChartDto(
    string Fecha,
    int Cantidad,
    decimal Porcentaje,
    int Semana,
    decimal Fcr);
