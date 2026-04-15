using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Inventario.Commands.EliminarMovimiento;

public class EliminarMovimientoCommandHandler : IRequestHandler<EliminarMovimientoCommand>
{
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGalponDbContext _context;

    public EliminarMovimientoCommandHandler(
        IInventarioRepository inventarioRepository, 
        IProductoRepository productoRepository,
        IUnitOfWork unitOfWork,
        IGalponDbContext context)
    {
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task Handle(EliminarMovimientoCommand request, CancellationToken cancellationToken)
    {
        var movimiento = await _context.ObtenerEntidadPorIdAsync<GalponERP.Domain.Entities.MovimientoInventario>(request.Id, cancellationToken);
        
        if (movimiento == null)
        {
            throw new Exception("El movimiento de inventario no existe.");
        }

        // Antes de desactivar, debemos reversar su impacto en el StockActualKg del producto
        var producto = await _productoRepository.ObtenerPorIdAsync(movimiento.ProductoId);
        if (producto != null)
        {
            // Para reversar una entrada (+), enviamos la cantidad con signo opuesto (-)
            // Para reversar una salida (-), enviamos la cantidad con signo opuesto (+)
            // Sin embargo, ActualizarStock ya maneja el factor por Tipo. 
            // Así que para REVERSAR, simplemente invertimos el factor del tipo.
            
            // Si era Entrada (factor 1), ahora factor -1.
            // Si era Salida (factor -1), ahora factor 1.
            // Una forma fácil es llamar a ActualizarStock con el tipo opuesto? No, mejor creamos un método ReversarStock o algo así.
            // O simplemente multiplicamos cantidad por -1.
            producto.ActualizarStock(-movimiento.Cantidad, movimiento.Tipo);
            _productoRepository.Actualizar(producto);
        }

        // Usamos Desactivar (Soft Delete) del Entity base
        movimiento.Desactivar();
        movimiento.SetAuditoriaModificacion(DateTime.UtcNow, request.UsuarioId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
