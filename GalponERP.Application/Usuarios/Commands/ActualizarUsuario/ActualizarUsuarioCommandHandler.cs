using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Usuarios.Commands.ActualizarUsuario;

public class ActualizarUsuarioCommandHandler : IRequestHandler<ActualizarUsuarioCommand>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarUsuarioCommandHandler(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ActualizarUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(request.Id);

        if (usuario == null)
        {
            throw new Exception("Usuario no encontrado.");
        }

        // Asegurar que la fecha sea UTC para PostgreSQL
        var fechaUtc = request.FechaNacimiento.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(request.FechaNacimiento, DateTimeKind.Utc) 
            : request.FechaNacimiento.ToUniversalTime();

        usuario.ActualizarPerfil(
            request.Email,
            request.Nombre,
            request.Apellidos,
            fechaUtc,
            request.Direccion,
            request.Profesion,
            request.Telefono,
            request.Rol);

        _usuarioRepository.Actualizar(usuario);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
