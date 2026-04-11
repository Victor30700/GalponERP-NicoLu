using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Lotes.Commands.EliminarLote;

public class EliminarLoteCommandHandler : IRequestHandler<EliminarLoteCommand, Unit>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarLoteCommandHandler(ILoteRepository loteRepository, IUnitOfWork unitOfWork)
    {
        _loteRepository = loteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(EliminarLoteCommand request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.Id);

        if (lote == null)
        {
            throw new Exception("Lote no encontrado.");
        }

        // Soft Delete
        _loteRepository.Eliminar(lote);
        lote.SetAuditoriaModificacion(DateTime.UtcNow, request.UsuarioId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
