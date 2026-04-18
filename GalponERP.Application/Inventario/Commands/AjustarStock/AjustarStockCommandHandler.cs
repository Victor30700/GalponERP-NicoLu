using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Exceptions;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Inventario.Commands;

public class AjustarStockCommandHandler : IRequestHandler<AjustarStockCommand, Guid>
{
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AjustarStockCommandHandler(
        IInventarioRepository inventarioRepository,
        IProductoRepository productoRepository,
        IUnitOfWork unitOfWork)
    {
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AjustarStockCommand request, CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.ObtenerPorIdAsync(request.ProductoId);
        if (producto == null)
            throw new InventarioDomainException("El producto no existe.");

        var stockSistema = await _inventarioRepository.ObtenerStockPorProductoIdAsync(request.ProductoId);
        decimal diferencia = request.CantidadFisica - stockSistema;

        if (diferencia == 0)
            return Guid.Empty; // No hubo cambio

        var tipoAjuste = diferencia > 0 ? TipoMovimiento.AjusteEntrada : TipoMovimiento.AjusteSalida;
        decimal cantidadAjuste = Math.Abs(diferencia);
        decimal costoTotalAjuste = cantidadAjuste * producto.CostoUnitarioActual;

        var movimientoId = Guid.NewGuid();
        var movimiento = new MovimientoInventario(
            movimientoId,
            request.ProductoId,
            null,
            cantidadAjuste,
            tipoAjuste,
            DateTime.UtcNow,
            request.UsuarioId,
            producto.PesoUnitarioKg,
            request.Nota ?? $"Ajuste de conciliación física (Sistema: {stockSistema}, Físico: {request.CantidadFisica})",
            new Moneda(costoTotalAjuste)
        );

        _inventarioRepository.RegistrarMovimiento(movimiento);

        // Actualizar stock en el Producto (cacheado)
        producto.ActualizarStock(cantidadAjuste, tipoAjuste);
        _productoRepository.Actualizar(producto);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return movimientoId;
    }
}
