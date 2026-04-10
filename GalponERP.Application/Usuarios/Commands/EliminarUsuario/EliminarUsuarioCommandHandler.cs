using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Usuarios.Commands.EliminarUsuario;

public class EliminarUsuarioCommandHandler : IRequestHandler<EliminarUsuarioCommand>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarUsuarioCommandHandler(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EliminarUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(request.Id);

        if (usuario == null)
        {
            throw new Exception($"Usuario con ID {request.Id} no encontrado.");
        }

        usuario.Desactivar();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
