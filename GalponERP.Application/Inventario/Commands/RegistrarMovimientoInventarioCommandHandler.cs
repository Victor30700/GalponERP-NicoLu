using GalponERP.Application.Interfaces;
using GalponERP.Application.Hubs;
using Microsoft.AspNetCore.SignalR;
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
    private readonly IHubContext<NotificationHub> _hubContext;

    public RegistrarMovimientoInventarioCommandHandler(
        IInventarioRepository inventarioRepository,
        IProductoRepository productoRepository,
        IUnitOfWork unitOfWork,
        IHubContext<NotificationHub> hubContext)
    {
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
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
            producto.PesoUnitarioKg, // Capturamos el peso actual como HISTÓRICO
            request.Justificacion,
            new Moneda(costoTotal)
        );

        _inventarioRepository.RegistrarMovimiento(movimiento);

        // Actualizar el stock en Kg cacheado en el Producto
        producto.ActualizarStock(request.Cantidad, request.Tipo);
        _productoRepository.Actualizar(producto);

        // Notificar si el stock baja del umbral crítico
        var nuevoStock = await _inventarioRepository.ObtenerStockPorProductoIdAsync(producto.Id);
        if (nuevoStock < producto.UmbralMinimo)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Inventario", 
                $"ALERTA DE STOCK: El producto {producto.Nombre} ha bajado de su mínimo. Stock actual: {nuevoStock} {producto.Unidad?.Nombre ?? "unidades"}", cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Inventario", "InventarioActualizado", cancellationToken);

        return movimientoId;
    }
}
