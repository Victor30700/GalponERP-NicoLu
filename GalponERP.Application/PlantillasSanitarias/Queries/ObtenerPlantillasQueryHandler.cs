using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.PlantillasSanitarias.Queries;

public record ObtenerPlantillasQuery() : IRequest<IEnumerable<PlantillaSanitariaDto>>;

public record PlantillaSanitariaDto(
    Guid Id,
    string Nombre,
    string? Descripcion,
    List<ActividadPlantillaDto> Actividades);

public record ActividadPlantillaDto(
    Guid Id,
    string TipoActividad,
    int DiaDeAplicacion,
    string Descripcion,
    Guid? ProductoIdRecomendado);

public class ObtenerPlantillasQueryHandler : IRequestHandler<ObtenerPlantillasQuery, IEnumerable<PlantillaSanitariaDto>>
{
    private readonly IPlantillaSanitariaRepository _plantillaRepository;

    public ObtenerPlantillasQueryHandler(IPlantillaSanitariaRepository plantillaRepository)
    {
        _plantillaRepository = plantillaRepository;
    }

    public async Task<IEnumerable<PlantillaSanitariaDto>> Handle(ObtenerPlantillasQuery request, CancellationToken cancellationToken)
    {
        var plantillas = await _plantillaRepository.ObtenerTodasAsync();

        return plantillas.Select(p => new PlantillaSanitariaDto(
            p.Id,
            p.Nombre,
            p.Descripcion,
            p.Actividades.Select(a => new ActividadPlantillaDto(
                a.Id,
                a.TipoActividad.ToString(),
                a.DiaDeAplicacion,
                a.Descripcion,
                a.ProductoIdRecomendado
            )).ToList()
        ));
    }
}
