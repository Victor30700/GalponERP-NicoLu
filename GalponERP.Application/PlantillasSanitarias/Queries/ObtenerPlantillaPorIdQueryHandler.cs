using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.PlantillasSanitarias.Queries;

public record ObtenerPlantillaPorIdQuery(Guid Id) : IRequest<PlantillaSanitariaDto?>;

public class ObtenerPlantillaPorIdQueryHandler : IRequestHandler<ObtenerPlantillaPorIdQuery, PlantillaSanitariaDto?>
{
    private readonly IPlantillaSanitariaRepository _plantillaRepository;

    public ObtenerPlantillaPorIdQueryHandler(IPlantillaSanitariaRepository plantillaRepository)
    {
        _plantillaRepository = plantillaRepository;
    }

    public async Task<PlantillaSanitariaDto?> Handle(ObtenerPlantillaPorIdQuery request, CancellationToken cancellationToken)
    {
        var p = await _plantillaRepository.ObtenerPorIdAsync(request.Id);
        if (p == null) return null;

        return new PlantillaSanitariaDto(
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
        );
    }
}
