using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Pesajes.Commands.EliminarPesaje;

public class EliminarPesajeCommandHandler : IRequestHandler<EliminarPesajeCommand, Unit>
{
    private readonly IPesajeLoteRepository _pesajeLoteRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarPesajeCommandHandler(
        IPesajeLoteRepository pesajeLoteRepository,
        ILoteRepository loteRepository,
        IUnitOfWork unitOfWork)
    {
        _pesajeLoteRepository = pesajeLoteRepository;
        _loteRepository = loteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(EliminarPesajeCommand request, CancellationToken cancellationToken)
    {
        var pesaje = await _pesajeLoteRepository.ObtenerPorIdAsync(request.Id);

        if (pesaje == null)
        {
            throw new Exception("Registro de pesaje no encontrado.");
        }

        var lote = await _loteRepository.ObtenerPorIdAsync(pesaje.LoteId);
        if (lote == null)
        {
            throw new Exception("Lote no encontrado.");
        }

        if (lote.Estado != Domain.Entities.EstadoLote.Activo)
        {
            throw new Exception("No se pueden eliminar pesajes de un lote cerrado o cancelado.");
        }

        // 1. Soft Delete
        _pesajeLoteRepository.Eliminar(pesaje);
        pesaje.SetAuditoriaModificacion(DateTime.UtcNow, request.UsuarioId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
