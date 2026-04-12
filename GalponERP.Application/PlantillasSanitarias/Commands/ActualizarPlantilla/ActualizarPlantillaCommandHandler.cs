using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.PlantillasSanitarias.Commands.CrearPlantilla;
using MediatR;

namespace GalponERP.Application.PlantillasSanitarias.Commands.ActualizarPlantilla;

public record ActualizarPlantillaCommand(
    Guid Id,
    string Nombre,
    string? Descripcion,
    List<PlantillaActividadCommand> Actividades) : IRequest;

public class ActualizarPlantillaCommandHandler : IRequestHandler<ActualizarPlantillaCommand>
{
    private readonly IPlantillaSanitariaRepository _plantillaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarPlantillaCommandHandler(IPlantillaSanitariaRepository plantillaRepository, IUnitOfWork unitOfWork)
    {
        _plantillaRepository = plantillaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ActualizarPlantillaCommand request, CancellationToken cancellationToken)
    {
        var plantilla = await _plantillaRepository.ObtenerPorIdAsync(request.Id);
        if (plantilla == null) throw new ArgumentException("Plantilla no encontrada.");

        plantilla.Actualizar(request.Nombre, request.Descripcion);
        plantilla.LimpiarActividades();

        foreach (var act in request.Actividades)
        {
            plantilla.AgregarActividad(Guid.NewGuid(), act.Tipo, act.DiaDeAplicacion, act.Descripcion, act.ProductoIdRecomendado);
        }

        _plantillaRepository.Actualizar(plantilla);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
