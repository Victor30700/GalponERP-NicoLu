using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Pesajes.Commands.ActualizarPesaje;

public class ActualizarPesajeCommandHandler : IRequestHandler<ActualizarPesajeCommand, Unit>
{
    private readonly IPesajeLoteRepository _pesajeLoteRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarPesajeCommandHandler(
        IPesajeLoteRepository pesajeLoteRepository,
        ILoteRepository loteRepository,
        IUnitOfWork unitOfWork)
    {
        _pesajeLoteRepository = pesajeLoteRepository;
        _loteRepository = loteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ActualizarPesajeCommand request, CancellationToken cancellationToken)
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
            throw new Exception("No se pueden editar pesajes de un lote cerrado o cancelado.");
        }

        // Asegurar que la fecha sea UTC para PostgreSQL
        var fechaUtc = request.Fecha.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(request.Fecha, DateTimeKind.Utc) 
            : request.Fecha.ToUniversalTime();

        // 1. Actualizar el registro de pesaje
        pesaje.Actualizar(fechaUtc, request.PesoPromedioGramos, request.CantidadMuestreada);
        pesaje.SetAuditoriaModificacion(DateTime.UtcNow, request.UsuarioId);

        // 2. Persistir
        _pesajeLoteRepository.Actualizar(pesaje);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
