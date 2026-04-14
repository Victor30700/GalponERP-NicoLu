using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Usuarios.Commands.RegistrarUsuario;

public class RegistrarUsuarioCommandHandler : IRequestHandler<RegistrarUsuarioCommand, Guid>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthenticationService _authenticationService;

    public RegistrarUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository, 
        IUnitOfWork unitOfWork,
        IAuthenticationService authenticationService)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _authenticationService = authenticationService;
    }

    public async Task<Guid> Handle(RegistrarUsuarioCommand request, CancellationToken cancellationToken)
    {
        // 1. Verificar si ya existe en la BD local por Email
        var existingUserByEmail = await _usuarioRepository.ObtenerPorEmailAsync(request.Email);
        if (existingUserByEmail != null)
        {
            return existingUserByEmail.Id;
        }

        // 2. Intentar crear o recuperar el UID de Firebase
        string firebaseUid;
        try
        {
            var extraData = new Dictionary<string, object>
            {
                { "nombre", request.Nombre },
                { "apellidos", request.Apellidos },
                { "rol", request.Rol.ToString() },
                { "direccion", request.Direccion ?? "" },
                { "profesion", request.Profesion ?? "" },
                { "fechaNacimiento", request.FechaNacimiento.ToString("yyyy-MM-dd") }
            };

            firebaseUid = await _authenticationService.CreateUserAsync(
                request.Email, 
                request.Password, 
                $"{request.Nombre} {request.Apellidos}",
                extraData);
        }
        catch (Exception ex) when (ex.Message.Contains("EMAIL_EXISTS"))
        {
            // El usuario ya existe en Firebase, recuperamos su UID
            var uid = await _authenticationService.GetUidByEmailAsync(request.Email);
            if (string.IsNullOrEmpty(uid))
            {
                throw new Exception("El usuario existe en Firebase pero no se pudo recuperar su UID.");
            }
            firebaseUid = uid;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error inesperado al gestionar usuario en Firebase: {ex.Message}");
        }

        // 3. Verificar si ya existe en la BD local por el UID recuperado (por si acaso cambió de email en Firebase)
        var existingUserByUid = await _usuarioRepository.ObtenerPorFirebaseUidAsync(firebaseUid);
        if (existingUserByUid != null)
        {
            return existingUserByUid.Id;
        }

        // 4. Crear usuario en la BD local
        // Asegurar que la fecha sea UTC para PostgreSQL
        var fechaUtc = request.FechaNacimiento.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(request.FechaNacimiento, DateTimeKind.Utc) 
            : request.FechaNacimiento.ToUniversalTime();

        var usuario = new Usuario(
            Guid.NewGuid(),
            firebaseUid,
            request.Email,
            request.Nombre,
            request.Apellidos,
            fechaUtc,
            request.Direccion ?? string.Empty,
            request.Profesion ?? string.Empty,
            request.Telefono ?? string.Empty,
            request.Rol);

        _usuarioRepository.Agregar(usuario);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return usuario.Id;
    }
}
