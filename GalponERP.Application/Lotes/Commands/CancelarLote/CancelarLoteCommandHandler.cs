using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Lotes.Commands.CancelarLote;

public record CancelarLoteCommand(Guid Id, string Justificacion) : IRequest;

public class CancelarLoteCommandHandler : IRequestHandler<CancelarLoteCommand>
{
    private readonly ILoteRepository _loteRepository;
    private readonly ICalendarioSanitarioRepository _calendarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelarLoteCommandHandler(
        ILoteRepository loteRepository, 
        ICalendarioSanitarioRepository calendarioRepository,
        IUnitOfWork unitOfWork)
    {
        _loteRepository = loteRepository;
        _calendarioRepository = calendarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CancelarLoteCommand request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.Id);
        if (lote == null) throw new ArgumentException("Lote no encontrado.");

        lote.Cancelar(request.Justificacion);

        // Inactivar calendario
        var calendario = await _calendarioRepository.ObtenerPorLoteIdAsync(lote.Id);
        foreach (var item in calendario)
        {
            item.Eliminar(); // Soft delete el item del calendario
            _calendarioRepository.Actualizar(item);
        }

        _loteRepository.Actualizar(lote);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
