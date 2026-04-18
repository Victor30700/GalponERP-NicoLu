using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Ventas.Commands.AnularPago;

public class AnularPagoVentaCommandHandler : IRequestHandler<AnularPagoVentaCommand>
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AnularPagoVentaCommandHandler(IVentaRepository ventaRepository, IUnitOfWork unitOfWork)
    {
        _ventaRepository = ventaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AnularPagoVentaCommand request, CancellationToken cancellationToken)
    {
        var venta = await _ventaRepository.ObtenerPorIdAsync(request.VentaId);

        if (venta == null)
            throw new InvalidOperationException("La venta no existe.");

        // Anulamos el pago usando la lógica de dominio
        venta.AnularPago(request.PagoId, request.UsuarioId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
