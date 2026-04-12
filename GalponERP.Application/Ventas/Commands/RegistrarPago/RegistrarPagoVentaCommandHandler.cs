using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Ventas.Commands.RegistrarPago;

public class RegistrarPagoVentaCommandHandler : IRequestHandler<RegistrarPagoVentaCommand, Guid>
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarPagoVentaCommandHandler(
        IVentaRepository ventaRepository,
        IUnitOfWork unitOfWork)
    {
        _ventaRepository = ventaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegistrarPagoVentaCommand request, CancellationToken cancellationToken)
    {
        var venta = await _ventaRepository.ObtenerPorIdAsync(request.VentaId);
        if (venta == null)
            throw new Exception($"Venta con ID {request.VentaId} no encontrada.");

        var pagoId = Guid.NewGuid();
        venta.RegistrarPago(
            pagoId,
            new Moneda(request.Monto),
            request.FechaPago,
            request.MetodoPago,
            request.UsuarioId);

        _ventaRepository.Actualizar(venta);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return pagoId;
    }
}
