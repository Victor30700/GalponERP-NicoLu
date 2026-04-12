using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.PlantillasSanitarias.Commands.EliminarPlantillaSanitaria;

public class EliminarPlantillaSanitariaCommandHandler : IRequestHandler<EliminarPlantillaSanitariaCommand>
{
    private readonly IPlantillaSanitariaRepository _plantillaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarPlantillaSanitariaCommandHandler(IPlantillaSanitariaRepository plantillaRepository, IUnitOfWork unitOfWork)
    {
        _plantillaRepository = plantillaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EliminarPlantillaSanitariaCommand request, CancellationToken cancellationToken)
    {
        var plantilla = await _plantillaRepository.ObtenerPorIdAsync(request.Id);

        if (plantilla == null)
        {
            throw new Exception("La plantilla no existe.");
        }

        plantilla.Eliminar(); // Soft Delete inherited from Entity
        _plantillaRepository.Actualizar(plantilla);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
