using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.Interfaces;
using GalponERP.Application.Exceptions;
using MediatR;

namespace GalponERP.Application.Configuracion.Commands.ActualizarConfiguracion;

public class ActualizarConfiguracionCommandHandler : IRequestHandler<ActualizarConfiguracionCommand>
{
    private readonly IConfiguracionRepository _configuracionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarConfiguracionCommandHandler(IConfiguracionRepository configuracionRepository, IUnitOfWork unitOfWork)
    {
        _configuracionRepository = configuracionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ActualizarConfiguracionCommand request, CancellationToken cancellationToken)
    {
        var config = await _configuracionRepository.ObtenerAsync();

        if (config == null)
        {
            // ...
        }
        else
        {
            if (!string.IsNullOrEmpty(request.Version) && config.Version.ToString() != request.Version)
            {
                throw new ConcurrencyException();
            }

            config.Actualizar(
                request.NombreEmpresa,
                request.Nit,
                request.Telefono,
                request.Email,
                request.Direccion,
                request.MonedaPorDefecto);
            
            if (!string.IsNullOrEmpty(request.LogoUrl))
                config.SetLogo(request.LogoUrl);

            _configuracionRepository.Actualizar(config);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
