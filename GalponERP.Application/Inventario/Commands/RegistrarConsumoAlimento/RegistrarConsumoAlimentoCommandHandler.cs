using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Exceptions;
using GalponERP.Domain.Interfaces.Repositories;
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

        // 1. Validar Stock Disponible
        var stockActual = await _inventarioRepository.ObtenerStockPorProductoIdAsync(request.ProductoId);
        if (stockActual < request.Cantidad)
        {
            throw new InventarioDomainException($"Stock insuficiente para el producto. Disponible: {stockActual}, Requerido: {request.Cantidad}");
        }

        // 2. Registrar el Movimiento de SALIDA
        // El sistema internamente calcula los Kg reales usando la Equivalencia del Producto
        // Aunque el Movimiento guarda la cantidad en su unidad (ej: bultos), 
        // los KPIs se calculan posteriormente usando esta relación.
        
        var movimientoId = Guid.NewGuid();
        var movimiento = new MovimientoInventario(
            movimientoId,
            request.ProductoId,
            request.LoteId,
            request.Cantidad,
            TipoMovimiento.Salida,
            DateTime.UtcNow,
            usuarioId.Value,
            request.Justificacion ?? "Consumo diario de alimento"
        );

        _inventarioRepository.RegistrarMovimiento(movimiento);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return movimientoId;
    }
}
