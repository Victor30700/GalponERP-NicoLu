using GalponERP.Application.Interfaces;
using GalponERP.Application.Exceptions;
using FluentValidation.Results;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Lotes.Commands.TrasladarLote;

public record TrasladarLoteCommand(Guid Id, Guid NuevoGalponId) : IRequest;

public class TrasladarLoteCommandHandler : IRequestHandler<TrasladarLoteCommand>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IGalponRepository _galponRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TrasladarLoteCommandHandler(ILoteRepository loteRepository, IGalponRepository galponRepository, IUnitOfWork unitOfWork)
    {
        _loteRepository = loteRepository;
        _galponRepository = galponRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(TrasladarLoteCommand request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.Id);
        if (lote == null) throw new KeyNotFoundException("Lote no encontrado.");

        // Validar que el nuevo galpón exista
        var galpon = await _galponRepository.ObtenerPorIdAsync(request.NuevoGalponId);
        if (galpon == null)
        {
            throw new ValidationException(new List<ValidationFailure> 
            { 
                new ValidationFailure("NuevoGalponId", $"El galpón con ID {request.NuevoGalponId} no existe.") 
            });
        }

        lote.Trasladar(request.NuevoGalponId);

        _loteRepository.Actualizar(lote);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
