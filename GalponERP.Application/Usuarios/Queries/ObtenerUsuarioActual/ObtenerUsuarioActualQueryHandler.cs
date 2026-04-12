using GalponERP.Application.Usuarios.Queries.ObtenerUsuarios;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Usuarios.Queries.ObtenerUsuarioActual;

public class ObtenerUsuarioActualQueryHandler : IRequestHandler<ObtenerUsuarioActualQuery, UsuarioResponse?>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public ObtenerUsuarioActualQueryHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<UsuarioResponse?> Handle(ObtenerUsuarioActualQuery request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(request.UsuarioId);
        
        if (usuario == null)
            return null;

        return new UsuarioResponse(
            usuario.Id,
            usuario.FirebaseUid,
            usuario.Email,
            usuario.Nombre,
            usuario.Rol,
            usuario.IsActive);
    }
}
