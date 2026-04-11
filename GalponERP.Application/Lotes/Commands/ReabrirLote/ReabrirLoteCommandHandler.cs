using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Lotes.Commands.ReabrirLote;

public class ReabrirLoteCommandHandler : IRequestHandler<ReabrirLoteCommand, Unit>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReabrirLoteCommandHandler(ILoteRepository loteRepository, IUnitOfWork unitOfWork)
    {
        _loteRepository = loteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ReabrirLoteCommand request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);

        if (lote == null)
        {
            throw new Exception("Lote no encontrado.");
        }

        lote.Reabrir();

        _loteRepository.Actualizar(lote);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
