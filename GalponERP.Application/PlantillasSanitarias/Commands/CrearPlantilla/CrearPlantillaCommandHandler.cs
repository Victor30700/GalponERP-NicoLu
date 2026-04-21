using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.PlantillasSanitarias.Commands.CrearPlantilla;

public record CrearPlantillaCommand(
    string Nombre,
    string? Descripcion,
    List<PlantillaActividadCommand> Actividades) : IRequest<Guid>, IAuditableCommand;

public record PlantillaActividadCommand(
    TipoActividad Tipo,
    int DiaDeAplicacion,
    string Descripcion,
    Guid? ProductoIdRecomendado);

public class CrearPlantillaCommandHandler : IRequestHandler<CrearPlantillaCommand, Guid>
{
    private readonly IPlantillaSanitariaRepository _plantillaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearPlantillaCommandHandler(IPlantillaSanitariaRepository plantillaRepository, IUnitOfWork unitOfWork)
    {
        _plantillaRepository = plantillaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CrearPlantillaCommand request, CancellationToken cancellationToken)
    {
        var plantilla = new PlantillaSanitaria(Guid.NewGuid(), request.Nombre, request.Descripcion);

        foreach (var act in request.Actividades)
        {
            plantilla.AgregarActividad(Guid.NewGuid(), act.Tipo, act.DiaDeAplicacion, act.Descripcion, act.ProductoIdRecomendado);
        }

        _plantillaRepository.Agregar(plantilla);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return plantilla.Id;
    }
}
