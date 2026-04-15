using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Sanidad.Commands.EliminarBienestar;

public class EliminarBienestarCommandHandler : IRequestHandler<EliminarBienestarCommand>
{
    private readonly IGalponDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarBienestarCommandHandler(IGalponDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EliminarBienestarCommand request, CancellationToken cancellationToken)
    {
        var registro = await _context.ObtenerEntidadPorIdAsync<GalponERP.Domain.Entities.RegistroBienestar>(request.Id, cancellationToken);
        
        if (registro == null)
        {
            throw new Exception("El registro de bienestar no existe.");
        }

        registro.Desactivar();
        registro.SetAuditoriaModificacion(DateTime.UtcNow, request.UsuarioId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
