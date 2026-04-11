using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Exceptions;
using MediatR;

namespace GalponERP.Application.Ventas.Commands.AnularVenta;

public class AnularVentaCommandHandler : IRequestHandler<AnularVentaCommand, Unit>
{
    private readonly IVentaRepository _ventaRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AnularVentaCommandHandler(
        IVentaRepository ventaRepository,
        ILoteRepository loteRepository,
        IUnitOfWork unitOfWork)
    {
        _ventaRepository = ventaRepository;
        _loteRepository = loteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AnularVentaCommand request, CancellationToken cancellationToken)
    {
        var venta = await _ventaRepository.ObtenerPorIdAsync(request.VentaId);
        if (venta == null || !venta.IsActive)
            throw new KeyNotFoundException($"Venta con ID {request.VentaId} no encontrada.");

        var lote = await _loteRepository.ObtenerPorIdAsync(venta.LoteId);
        if (lote == null)
            throw new KeyNotFoundException($"Lote con ID {venta.LoteId} no encontrado.");

        if (lote.Estado == EstadoLote.Cerrado)
            throw new LoteDomainException("No se puede anular una venta de un lote que ya está cerrado.");

        // 1. Marcar venta como inactiva (Soft Delete)
        venta.Eliminar();
        venta.SetAuditoriaModificacion(DateTime.UtcNow, request.UsuarioId);

        // 2. Devolver la cantidad de pollos al lote
        lote.AnularVenta(venta.CantidadPollos);

        // 3. Persistir cambios de manera atómica
        _ventaRepository.Actualizar(venta);
        _loteRepository.Actualizar(lote);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
