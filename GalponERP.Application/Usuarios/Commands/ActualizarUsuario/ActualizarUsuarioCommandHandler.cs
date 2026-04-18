using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.Exceptions;
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

        // Verificar duplicidad de teléfono si se está cambiando
        string? telefonoNormalizado = request.Telefono?.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        if (!string.IsNullOrWhiteSpace(telefonoNormalizado))
        {
            var existingUserByPhone = await _usuarioRepository.ObtenerPorTelefonoAsync(telefonoNormalizado);
            if (existingUserByPhone != null && existingUserByPhone.Id != usuario.Id)
            {
                throw new UsuarioDomainException($"El número de teléfono {telefonoNormalizado} ya está registrado con otro usuario.");
            }
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
            telefonoNormalizado,
            request.Rol);

        usuario.ActualizarEstado(request.Active);

        _usuarioRepository.Actualizar(usuario);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
