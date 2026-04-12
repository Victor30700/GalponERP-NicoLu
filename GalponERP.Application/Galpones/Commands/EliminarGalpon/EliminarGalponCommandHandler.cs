using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Galpones.Commands.EliminarGalpon;

public class EliminarGalponCommandHandler : IRequestHandler<EliminarGalponCommand>
{
    private readonly IGalponRepository _galponRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarGalponCommandHandler(IGalponRepository galponRepository, IUnitOfWork unitOfWork)
    {
        _galponRepository = galponRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EliminarGalponCommand request, CancellationToken cancellationToken)
    {
        var galpon = await _galponRepository.ObtenerPorIdAsync(request.Id);

        if (galpon == null)
        {
            throw new Exception("El galpón no existe.");
        }

        galpon.Eliminar();
        _galponRepository.Actualizar(galpon);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
