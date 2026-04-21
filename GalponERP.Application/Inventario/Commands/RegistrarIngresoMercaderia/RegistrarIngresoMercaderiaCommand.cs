using FluentValidation;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using MediatR;
using System.Text.Json.Serialization;

namespace GalponERP.Application.Inventario.Commands.RegistrarIngresoMercaderia;

public record RegistrarIngresoMercaderiaCommand(
    Guid ProductoId,
    decimal Cantidad,
    decimal CostoTotalCompra,
    Guid ProveedorId,
    decimal MontoPagado,
    string? NumeroLoteFabricante = null,
    string? FechaVencimiento = null,
    string? Nota = null) : IRequest<Guid>
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}

public class RegistrarIngresoMercaderiaCommandValidator : AbstractValidator<RegistrarIngresoMercaderiaCommand>
{
    public RegistrarIngresoMercaderiaCommandValidator()
    {
        RuleFor(x => x.ProductoId).NotEmpty();
        RuleFor(x => x.Cantidad).GreaterThan(0);
        RuleFor(x => x.CostoTotalCompra).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ProveedorId).NotEmpty();
        RuleFor(x => x.MontoPagado).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MontoPagado).LessThanOrEqualTo(x => x.CostoTotalCompra)
            .WithMessage("El monto pagado no puede ser mayor al costo total de la compra.");
    }
}

public class RegistrarIngresoMercaderiaCommandHandler : IRequestHandler<RegistrarIngresoMercaderiaCommand, Guid>
{
    private readonly IInventarioRepository _inventarioRepository;
    private readonly ICompraInventarioRepository _compraRepository;
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarIngresoMercaderiaCommandHandler(
        IInventarioRepository inventarioRepository, 
        ICompraInventarioRepository compraRepository,
        IProveedorRepository proveedorRepository,
        IProductoRepository productoRepository,
        IUnitOfWork unitOfWork)
    {
        _inventarioRepository = inventarioRepository;
        _compraRepository = compraRepository;
        _proveedorRepository = proveedorRepository;
        _productoRepository = productoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegistrarIngresoMercaderiaCommand request, CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.ObtenerPorIdAsync(request.ProductoId);
        if (producto == null)
        {
            throw new Exception("El producto especificado no existe.");
        }

        var proveedor = await _proveedorRepository.ObtenerPorIdAsync(request.ProveedorId);
        if (proveedor == null)
        {
            throw new Exception("El proveedor especificado no existe.");
        }

        // 1. Recalcular Costo PPP
        decimal stockActual = await _inventarioRepository.ObtenerStockPorProductoIdAsync(request.ProductoId);
        decimal precioUnitarioCompra = request.Cantidad > 0 ? request.CostoTotalCompra / request.Cantidad : 0;
        
        producto.RecalcularCostoPPP(stockActual, request.Cantidad, precioUnitarioCompra);
        _productoRepository.Actualizar(producto);

        // 2. Crear la Compra
        var compraId = Guid.NewGuid();
        var compra = new CompraInventario(
            compraId,
            request.ProveedorId,
            DateTime.UtcNow,
            new Moneda(request.CostoTotalCompra),
            new Moneda(request.MontoPagado),
            request.UsuarioId,
            request.Nota);

        _compraRepository.Agregar(compra);

        // 3. Crear Lote de Fabricante si se proporciona
        Guid? inventarioLoteId = null;
        if (!string.IsNullOrWhiteSpace(request.NumeroLoteFabricante) && !string.IsNullOrWhiteSpace(request.FechaVencimiento))
        {
            if (DateTime.TryParse(request.FechaVencimiento, out DateTime vencimiento))
            {
                var nuevoLote = new InventarioLote(
                    Guid.NewGuid(),
                    request.ProductoId,
                    request.NumeroLoteFabricante,
                    vencimiento,
                    request.Cantidad,
                    DateTime.UtcNow
                );
                _inventarioRepository.RegistrarLote(nuevoLote);
                inventarioLoteId = nuevoLote.Id;
            }
        }

        // 4. Crear el Movimiento de Inventario (Kárdex)
        var movimiento = new MovimientoInventario(
            Guid.NewGuid(),
            request.ProductoId,
            null,
            request.Cantidad,
            TipoMovimiento.Compra,
            DateTime.UtcNow,
            request.UsuarioId,
            producto.PesoUnitarioKg, // Capturamos el peso actual como HISTÓRICO
            request.Nota,
            new Moneda(request.CostoTotalCompra),
            proveedor.RazonSocial,
            compraId,
            inventarioLoteId);

        _inventarioRepository.RegistrarMovimiento(movimiento);

        // 5. Actualizar el stock en Kg cacheado en el Producto
        producto.ActualizarStock(request.Cantidad, TipoMovimiento.Compra);
        _productoRepository.Actualizar(producto);

        // 6. Persistir todo en una sola transacción
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return compra.Id;
    }
}
