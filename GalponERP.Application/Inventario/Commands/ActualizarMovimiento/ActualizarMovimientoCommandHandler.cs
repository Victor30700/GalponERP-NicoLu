using GalponERP.Application.Interfaces;
using GalponERP.Application.Exceptions;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Inventario.Commands.ActualizarMovimiento;

public class ActualizarMovimientoCommandHandler : IRequestHandler<ActualizarMovimientoCommand>
{
    private readonly IGalponDbContext _context;
    private readonly IProductoRepository _productoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarMovimientoCommandHandler(
        IGalponDbContext context, 
        IProductoRepository productoRepository,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _productoRepository = productoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ActualizarMovimientoCommand request, CancellationToken cancellationToken)
    {
        var movimiento = await _context.ObtenerEntidadPorIdAsync<GalponERP.Domain.Entities.MovimientoInventario>(request.Id, cancellationToken);
        
        if (movimiento == null)
        {
            throw new Exception("El movimiento de inventario no existe.");
        }

        // Chequeo de concurrencia optimista
        if (!string.IsNullOrEmpty(request.Version) && movimiento.Version.ToString() != request.Version)
        {
            throw new ConcurrencyException();
        }

        var producto = await _productoRepository.ObtenerPorIdAsync(request.ProductoId);
        if (producto == null)
        {
            throw new Exception("El producto no existe.");
        }

        // 1. Reversar el impacto anterior en el StockActualKg del producto original
        if (movimiento.ProductoId == request.ProductoId)
        {
            // Mismo producto, solo ajustamos la diferencia
            producto.ActualizarStock(-movimiento.Cantidad, movimiento.Tipo);
            producto.ActualizarStock(request.Cantidad, movimiento.Tipo);
        }
        else
        {
            // Diferente producto, reversamos el viejo y aplicamos al nuevo
            var productoAnterior = await _productoRepository.ObtenerPorIdAsync(movimiento.ProductoId);
            if (productoAnterior != null)
            {
                productoAnterior.ActualizarStock(-movimiento.Cantidad, movimiento.Tipo);
                _productoRepository.Actualizar(productoAnterior);
            }
            producto.ActualizarStock(request.Cantidad, movimiento.Tipo);
        }

        decimal costoTotalConsumo = request.Cantidad * producto.CostoUnitarioActual;

        movimiento.Actualizar(
            request.ProductoId,
            request.Cantidad,
            request.Fecha,
            request.Justificacion,
            new Moneda(costoTotalConsumo)
        );

        _productoRepository.Actualizar(producto);
        movimiento.SetAuditoriaModificacion(DateTime.UtcNow, request.UsuarioId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
