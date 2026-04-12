using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Inventario.Commands.RegistrarPagoCompra;

public class RegistrarPagoCompraCommandHandler : IRequestHandler<RegistrarPagoCompraCommand, Guid>
{
    private readonly ICompraInventarioRepository _compraRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarPagoCompraCommandHandler(
        ICompraInventarioRepository compraRepository,
        IUnitOfWork unitOfWork)
    {
        _compraRepository = compraRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegistrarPagoCompraCommand request, CancellationToken cancellationToken)
    {
        var compra = await _compraRepository.ObtenerPorIdAsync(request.CompraId);
        if (compra == null)
            throw new Exception($"Compra con ID {request.CompraId} no encontrada.");

        var pagoId = Guid.NewGuid();
        compra.RegistrarPago(
            pagoId,
            new Moneda(request.Monto),
            request.FechaPago,
            request.MetodoPago,
            request.UsuarioId);

        _compraRepository.Actualizar(compra);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return pagoId;
    }
}
