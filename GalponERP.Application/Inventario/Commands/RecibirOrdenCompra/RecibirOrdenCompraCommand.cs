using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Inventario.Commands.RecibirOrdenCompra;

public record RecibirOrdenCompraCommand(
    Guid OrdenCompraId,
    decimal MontoPagado,
    string? NotaRecibo = null) : IRequest<Guid>, IAuditableCommand;

public class RecibirOrdenCompraCommandHandler : IRequestHandler<RecibirOrdenCompraCommand, Guid>
{
    private readonly IOrdenCompraRepository _ordenCompraRepository;
    private readonly IInventarioRepository _inventarioRepository;
    private readonly ICompraInventarioRepository _compraRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUserContext;

    public RecibirOrdenCompraCommandHandler(
        IOrdenCompraRepository ordenCompraRepository,
        IInventarioRepository inventarioRepository,
        ICompraInventarioRepository compraRepository,
        IProductoRepository productoRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserContext currentUserContext)
    {
        _ordenCompraRepository = ordenCompraRepository;
        _inventarioRepository = inventarioRepository;
        _compraRepository = compraRepository;
        _productoRepository = productoRepository;
        _unitOfWork = unitOfWork;
        _currentUserContext = currentUserContext;
    }

    public async Task<Guid> Handle(RecibirOrdenCompraCommand request, CancellationToken cancellationToken)
    {
        var oc = await _ordenCompraRepository.ObtenerPorIdAsync(request.OrdenCompraId);
        if (oc == null) throw new Exception("Orden de compra no encontrada.");
        if (oc.Estado != EstadoOrdenCompra.Pendiente) throw new Exception("La orden de compra no está en estado pendiente.");

        var usuarioId = _currentUserContext.UsuarioId ?? Guid.Empty;

        // 1. Crear Compra formal
        var compraId = Guid.NewGuid();
        var compra = new CompraInventario(
            compraId,
            oc.ProveedorId,
            DateTime.UtcNow,
            oc.Total,
            new Moneda(request.MontoPagado),
            usuarioId,
            request.NotaRecibo ?? oc.Nota);

        _compraRepository.Agregar(compra);

        // 2. Procesar ítems (Kárdex y PPP)
        foreach (var item in oc.Items)
        {
            var producto = await _productoRepository.ObtenerPorIdAsync(item.ProductoId);
            if (producto == null) continue;

            // Recalcular PPP
            decimal stockActual = await _inventarioRepository.ObtenerStockPorProductoIdAsync(item.ProductoId);
            producto.RecalcularCostoPPP(stockActual, item.Cantidad, item.PrecioUnitario.Monto);
            _productoRepository.Actualizar(producto);

            // Registrar Movimiento
            var movimiento = new MovimientoInventario(
                Guid.NewGuid(),
                item.ProductoId,
                null,
                item.Cantidad,
                TipoMovimiento.Compra,
                DateTime.UtcNow,
                usuarioId,
                producto.PesoUnitarioKg, // Capturamos el peso actual como HISTÓRICO
                $"Recepción OC {oc.Id}: {request.NotaRecibo}",
                item.Total,
                oc.Proveedor?.RazonSocial,
                compraId);

            _inventarioRepository.RegistrarMovimiento(movimiento);

            // 3. Actualizar el stock en Kg cacheado en el Producto
            producto.ActualizarStock(item.Cantidad, TipoMovimiento.Compra);
            _productoRepository.Actualizar(producto);
        }

        // 4. Cerrar OC
        oc.MarcarComoRecibida();
        _ordenCompraRepository.Actualizar(oc);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return compra.Id;
    }
}
