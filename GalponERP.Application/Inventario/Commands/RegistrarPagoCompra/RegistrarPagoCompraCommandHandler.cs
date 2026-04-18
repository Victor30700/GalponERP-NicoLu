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
        
        // Asegurar que la fecha sea UTC para PostgreSQL
        var fechaUtc = request.FechaPago.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(request.FechaPago, DateTimeKind.Utc) 
            : request.FechaPago.ToUniversalTime();

        compra.RegistrarPago(
            pagoId,
            new Moneda(request.Monto),
            fechaUtc,
            request.MetodoPago,
            request.UsuarioId);

        // Al estar la entidad rastreada, no es necesario llamar a Actualizar.
        // Esto previene errores de concurrencia al actualizar el grafo de pagos.

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return pagoId;
    }
}
