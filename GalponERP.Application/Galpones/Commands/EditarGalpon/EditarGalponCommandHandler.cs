using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Galpones.Commands.EditarGalpon;

public class EditarGalponCommandHandler : IRequestHandler<EditarGalponCommand>
{
    private readonly IGalponRepository _galponRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EditarGalponCommandHandler(IGalponRepository galponRepository, IUnitOfWork unitOfWork)
    {
        _galponRepository = galponRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EditarGalponCommand request, CancellationToken cancellationToken)
    {
        var galpon = await _galponRepository.ObtenerPorIdAsync(request.Id);

        if (galpon == null)
        {
            throw new KeyNotFoundException($"Galpón con ID {request.Id} no encontrado.");
        }

        galpon.Actualizar(request.Nombre, request.Capacidad, request.Ubicacion);

        _galponRepository.Actualizar(galpon);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
