using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Inventario.Commands.RegistrarMovimiento;

public record RegistrarMovimientoInventarioCommand(
    Guid ProductoId,
    Guid? LoteId,
    decimal Cantidad,
    TipoMovimiento Tipo,
    DateTime Fecha,
    string? Justificacion = null) : IRequest<Guid>;

public class RegistrarMovimientoInventarioCommandHandler : IRequestHandler<RegistrarMovimientoInventarioCommand, Guid>
{
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarMovimientoInventarioCommandHandler(IInventarioRepository inventarioRepository, IUnitOfWork unitOfWork)
    {
        _inventarioRepository = inventarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegistrarMovimientoInventarioCommand request, CancellationToken cancellationToken)
    {
        var movimiento = new MovimientoInventario(
            Guid.NewGuid(),
            request.ProductoId,
            request.LoteId,
            request.Cantidad,
            request.Tipo,
            request.Fecha,
            request.Justificacion);

        _inventarioRepository.RegistrarMovimiento(movimiento);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return movimiento.Id;
    }
}
