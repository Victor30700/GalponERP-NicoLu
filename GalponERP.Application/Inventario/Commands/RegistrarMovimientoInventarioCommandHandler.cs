using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Inventario.Commands.RegistrarMovimiento;

public class RegistrarMovimientoInventarioCommandHandler : IRequestHandler<RegistrarMovimientoInventarioCommand, Guid>
{
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarMovimientoInventarioCommandHandler(
        IInventarioRepository inventarioRepository,
        IProductoRepository productoRepository,
        IUnitOfWork unitOfWork)
    {
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegistrarMovimientoInventarioCommand request, CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.ObtenerPorIdAsync(request.ProductoId);
        if (producto == null)
        {
            throw new Exception("El producto no existe.");
        }

        decimal costoTotal = request.Cantidad * producto.CostoUnitarioActual;

        var movimientoId = Guid.NewGuid();
        var movimiento = new MovimientoInventario(
            movimientoId,
            request.ProductoId,
            request.LoteId,
            request.Cantidad,
            request.Tipo,
            request.Fecha,
            request.UsuarioId,
            request.Justificacion,
            new Moneda(costoTotal)
        );

        _inventarioRepository.RegistrarMovimiento(movimiento);

        // Actualizar el stock en Kg cacheado en el Producto
        producto.ActualizarStock(request.Cantidad, request.Tipo);
        _productoRepository.Actualizar(producto);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return movimientoId;
    }
}
