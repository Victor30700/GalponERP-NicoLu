using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Usuarios.Commands.CambiarRolUsuario;

public record CambiarRolUsuarioCommand(Guid UsuarioId, RolGalpon NuevoRol) : IRequest<Unit>, IAuditableCommand;

public class CambiarRolUsuarioCommandHandler : IRequestHandler<CambiarRolUsuarioCommand, Unit>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CambiarRolUsuarioCommandHandler(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CambiarRolUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(request.UsuarioId);
        if (usuario == null)
            throw new Exception("Usuario no encontrado.");

        usuario.ActualizarRol(request.NuevoRol);
        _usuarioRepository.Actualizar(usuario);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
