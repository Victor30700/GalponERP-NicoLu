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
    private readonly ILoteRepository _loteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUserContext;

    public RegistrarConsumoAlimentoCommandHandler(
        IInventarioRepository inventarioRepository,
        IProductoRepository productoRepository,
        ILoteRepository loteRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserContext currentUserContext)
    {
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _loteRepository = loteRepository;
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

        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null)
        {
            throw new Exception("El lote no existe.");
        }

        var usuarioId = _currentUserContext.UsuarioId;
        if (!usuarioId.HasValue || usuarioId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Usuario no identificado para realizar la operación.");
        }

        // 1. Validar Stock Disponible
        // El frontend envía la cantidad en Kilogramos para mayor precisión en el FCR.
        // Convertimos a unidades para validar contra el stock y registrar el movimiento.
        decimal kgConsumidos = request.Cantidad;
        decimal unidadesAConsumir = producto.CalcularUnidades(kgConsumidos);

        // Caso especial: si el producto no tiene peso unitario (ej: 0), pero se intenta registrar consumo por Kg, 
        // asumimos que 1 unidad = 1 Kg si no hay conversión definida, para no romper la lógica anterior.
        // Sin embargo, lo ideal es que Alimento SIEMPRE tenga peso unitario > 0.
        if (producto.PesoUnitarioKg == 0 && kgConsumidos > 0)
        {
            unidadesAConsumir = kgConsumidos;
        }

        var stockActualUnidades = await _inventarioRepository.ObtenerStockPorProductoIdAsync(request.ProductoId);
        
        if (stockActualUnidades < unidadesAConsumir)
        {
            throw new InventarioDomainException($"Stock insuficiente. Disponible: {stockActualUnidades} {producto.Unidad?.Nombre ?? "unidades"}, Requerido: {unidadesAConsumir} (equiv. a {kgConsumidos} Kg)");
        }

        // 2. Registrar el Movimiento de SALIDA en UNIDADES (Calculadas desde Kg)
        decimal costoTotalConsumo = unidadesAConsumir * producto.CostoUnitarioActual;

        var movimientoId = Guid.NewGuid();
        var movimiento = new MovimientoInventario(
            movimientoId,
            request.ProductoId,
            request.LoteId,
            unidadesAConsumir, // Se guardan Unidades con alta precisión decimal
            TipoMovimiento.Salida,
            DateTime.UtcNow,
            usuarioId.Value,
            producto.PesoUnitarioKg, // Capturamos el peso actual como HISTÓRICO
            request.Justificacion ?? $"Consumo de {kgConsumidos} Kg ({unidadesAConsumir} {producto.Unidad?.Nombre ?? "unidades"})",
            new Moneda(costoTotalConsumo)
        );

        _inventarioRepository.RegistrarMovimiento(movimiento);

        // 3. Actualizar el stock en Kg cacheado en el Producto
        producto.ActualizarStock(unidadesAConsumir, TipoMovimiento.Salida);
        _productoRepository.Actualizar(producto);

        // Blindaje Fase 1: Si el producto tiene periodo de retiro, actualizar el lote
        if (producto.PeriodoRetiroDias > 0)
        {
            lote.RegistrarAplicacionMedica(DateTime.UtcNow, producto.PeriodoRetiroDias);
            _loteRepository.Actualizar(lote);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return movimientoId;
    }
}
