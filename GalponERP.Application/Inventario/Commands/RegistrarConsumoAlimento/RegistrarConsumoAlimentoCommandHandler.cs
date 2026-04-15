using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Exceptions;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Inventario.Commands.RegistrarConsumoAlimento;

public class RegistrarConsumoAlimentoCommandHandler : IRequestHandler<RegistrarConsumoAlimentoCommand, Guid>
{
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUserContext;

    public RegistrarConsumoAlimentoCommandHandler(
        IInventarioRepository inventarioRepository,
        IProductoRepository productoRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserContext currentUserContext)
    {
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _unitOfWork = unitOfWork;
        _currentUserContext = currentUserContext;
    }

    public async Task<Guid> Handle(RegistrarConsumoAlimentoCommand request, CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.ObtenerPorIdAsync(request.ProductoId);
        if (producto == null)
        {
            throw new Exception("El producto no existe.");
        }

        var usuarioId = _currentUserContext.UsuarioId;
        if (!usuarioId.HasValue || usuarioId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Usuario no identificado para realizar la operación.");
        }

        // 1. Validar Stock Disponible (Se valida contra lo que devuelva el repositorio en unidades)
        var stockActualUnidades = await _inventarioRepository.ObtenerStockPorProductoIdAsync(request.ProductoId);
        
        if (stockActualUnidades < request.Cantidad)
        {
            throw new InventarioDomainException($"Stock insuficiente. Disponible: {stockActualUnidades} {producto.Unidad?.Nombre ?? "unidades"}, Requerido: {request.Cantidad}");
        }

        // 2. Registrar el Movimiento de SALIDA en UNIDADES
        decimal costoTotalConsumo = request.Cantidad * producto.CostoUnitarioActual;
        decimal kgConsumidos = request.Cantidad * producto.PesoUnitarioKg;

        var movimientoId = Guid.NewGuid();
        var movimiento = new MovimientoInventario(
            movimientoId,
            request.ProductoId,
            request.LoteId,
            request.Cantidad, // Se guardan Unidades (ej: 5 sacos)
            TipoMovimiento.Salida,
            DateTime.UtcNow,
            usuarioId.Value,
            request.Justificacion ?? $"Consumo de {request.Cantidad} {producto.Unidad?.Nombre ?? "unidades"} ({kgConsumidos} Kg)",
            new Moneda(costoTotalConsumo)
        );

        _inventarioRepository.RegistrarMovimiento(movimiento);

        // 3. Actualizar el stock en Kg cacheado en el Producto
        // ActualizarStock recibe unidades y multiplica internamente por PesoUnitarioKg
        producto.ActualizarStock(request.Cantidad, TipoMovimiento.Salida);
        _productoRepository.Actualizar(producto);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return movimientoId;
    }
}
