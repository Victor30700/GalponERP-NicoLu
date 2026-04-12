using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Lotes.Commands.TrasladarLote;

public record TrasladarLoteCommand(Guid Id, Guid NuevoGalponId) : IRequest;

public class TrasladarLoteCommandHandler : IRequestHandler<TrasladarLoteCommand>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TrasladarLoteCommandHandler(ILoteRepository loteRepository, IUnitOfWork unitOfWork)
    {
        _loteRepository = loteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(TrasladarLoteCommand request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.Id);
        if (lote == null) throw new ArgumentException("Lote no encontrado.");

        lote.Trasladar(request.NuevoGalponId);

        _loteRepository.Actualizar(lote);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
