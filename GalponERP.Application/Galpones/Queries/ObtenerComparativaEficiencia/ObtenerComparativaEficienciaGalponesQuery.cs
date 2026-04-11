using MediatR;

namespace GalponERP.Application.Galpones.Queries.ObtenerComparativaEficiencia;

public record ObtenerComparativaEficienciaGalponesQuery() : IRequest<List<EficienciaGalponDto>>;

public record EficienciaGalponDto(
    Guid GalponId,
    string Nombre,
    int TotalLotes,
    decimal PromedioMortalidad,
    decimal PromedioFCR,
    decimal UtilidadTotalAcumulada);
