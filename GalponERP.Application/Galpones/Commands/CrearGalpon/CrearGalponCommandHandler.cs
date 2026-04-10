using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Galpones.Commands.CrearGalpon;

public class CrearGalponCommandHandler : IRequestHandler<CrearGalponCommand, Guid>
{
    private readonly IGalponRepository _galponRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearGalponCommandHandler(IGalponRepository galponRepository, IUnitOfWork unitOfWork)
    {
        _galponRepository = galponRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CrearGalponCommand request, CancellationToken cancellationToken)
    {
        var galpon = new Galpon(
            Guid.NewGuid(),
            request.Nombre,
            request.Capacidad,
            request.Ubicacion);

        _galponRepository.Agregar(galpon);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return galpon.Id;
    }
}
