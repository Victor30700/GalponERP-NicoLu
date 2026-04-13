using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Usuarios.Queries.ObtenerUsuarios;

public class ObtenerUsuariosQueryHandler : IRequestHandler<ObtenerUsuariosQuery, IEnumerable<UsuarioResponse>>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public ObtenerUsuariosQueryHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<IEnumerable<UsuarioResponse>> Handle(ObtenerUsuariosQuery request, CancellationToken cancellationToken)
    {
        var usuarios = await _usuarioRepository.ObtenerTodosConInactivosAsync();
        
        return usuarios.Select(u => new UsuarioResponse(
            u.Id,
            u.FirebaseUid,
            u.Email,
            u.Nombre,
            u.Rol,
            u.IsActive));
    }
}
