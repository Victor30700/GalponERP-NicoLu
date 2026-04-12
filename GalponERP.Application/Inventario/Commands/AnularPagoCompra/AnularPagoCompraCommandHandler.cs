using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Inventario.Commands.AnularPagoCompra;

public class AnularPagoCompraCommandHandler : IRequestHandler<AnularPagoCompraCommand>
{
    private readonly ICompraInventarioRepository _compraRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AnularPagoCompraCommandHandler(
        ICompraInventarioRepository compraRepository,
        IUnitOfWork unitOfWork)
    {
        _compraRepository = compraRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AnularPagoCompraCommand request, CancellationToken cancellationToken)
    {
        var compra = await _compraRepository.ObtenerPorIdAsync(request.CompraId);
        if (compra == null)
            throw new Exception($"Compra con ID {request.CompraId} no encontrada.");

        compra.AnularPago(request.PagoId);
        compra.SetAuditoriaModificacion(DateTime.UtcNow, request.UsuarioId);

        _compraRepository.Actualizar(compra);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
