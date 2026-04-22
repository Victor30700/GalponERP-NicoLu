using GalponERP.Application.Interfaces;
using GalponERP.Application.Exceptions;
using MediatR;

namespace GalponERP.Application.Sanidad.Commands.ActualizarBienestar;

public class ActualizarBienestarCommandHandler : IRequestHandler<ActualizarBienestarCommand>
{
    private readonly IGalponDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarBienestarCommandHandler(IGalponDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ActualizarBienestarCommand request, CancellationToken cancellationToken)
    {
        var registro = await _context.ObtenerEntidadPorIdAsync<GalponERP.Domain.Entities.RegistroBienestar>(request.Id, cancellationToken);
        
        if (registro == null)
        {
            throw new Exception("El registro de bienestar no existe.");
        }

        if (registro.Version.ToString() != request.Version)
        {
            throw new ConcurrencyException();
        }

        registro.Actualizar(
            request.Temperatura,
            request.Humedad,
            request.ConsumoAgua,
            request.Observaciones
        );

        registro.SetAuditoriaModificacion(DateTime.UtcNow, request.UsuarioId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
