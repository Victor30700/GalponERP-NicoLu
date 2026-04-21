using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.PlantillasSanitarias.Commands.CrearPlantilla;
using MediatR;

namespace GalponERP.Application.PlantillasSanitarias.Commands.ActualizarPlantilla;

public record ActualizarPlantillaCommand(
    Guid Id,
    string Nombre,
    string? Descripcion,
    List<PlantillaActividadCommand> Actividades) : IRequest, IAuditableCommand;

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
        if (plantilla == null) throw new ArgumentException("Plantilla no encontrada o ya no está activa.");

        // Actualizar datos básicos
        plantilla.Actualizar(request.Nombre, request.Descripcion);
        
        // Al limpiar y luego agregar con Guid.NewGuid(), EF Core maneja el reemplazo de la colección.
        // Desactivamos las actividades existentes (Soft Delete)
        plantilla.LimpiarActividades();

        // Agregamos las nuevas actividades del comando
        if (request.Actividades != null)
        {
            foreach (var act in request.Actividades)
            {
                plantilla.AgregarActividad(Guid.NewGuid(), act.Tipo, act.DiaDeAplicacion, act.Descripcion, act.ProductoIdRecomendado);
            }
        }

        try 
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Capturar otros errores potenciales para facilitar la depuración
            var innerMessage = ex.InnerException?.Message ?? "";
            throw new Exception($"Error al actualizar la plantilla: {ex.Message}. {innerMessage}", ex);
        }
    }
}
