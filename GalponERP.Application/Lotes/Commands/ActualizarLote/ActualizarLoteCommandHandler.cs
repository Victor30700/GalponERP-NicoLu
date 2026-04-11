using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Lotes.Commands.ActualizarLote;

public class ActualizarLoteCommandHandler : IRequestHandler<ActualizarLoteCommand, Unit>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarLoteCommandHandler(ILoteRepository loteRepository, IUnitOfWork unitOfWork)
    {
        _loteRepository = loteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ActualizarLoteCommand request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.Id);

        if (lote == null)
        {
            throw new Exception("Lote no encontrado.");
        }

        // 1. Actualizar datos iniciales (el método en la entidad valida el estado Activo)
        lote.ActualizarDatosIniciales(
            request.GalponId,
            request.FechaIngreso,
            request.CantidadInicial,
            new Moneda(request.CostoUnitarioPollito)
        );

        lote.SetAuditoriaModificacion(DateTime.UtcNow, request.UsuarioId);

        // 2. Persistir
        _loteRepository.Actualizar(lote);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
