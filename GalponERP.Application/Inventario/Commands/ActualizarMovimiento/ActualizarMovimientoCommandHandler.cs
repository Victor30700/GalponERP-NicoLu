using GalponERP.Application.Interfaces;
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

        var producto = await _productoRepository.ObtenerPorIdAsync(request.ProductoId);
        if (producto == null)
        {
            throw new Exception("El producto no existe.");
        }

        decimal costoTotalConsumo = request.Cantidad * producto.CostoUnitarioActual;

        movimiento.Actualizar(
            request.ProductoId,
            request.Cantidad,
            request.Fecha,
            request.Justificacion,
            new Moneda(costoTotalConsumo)
        );

        movimiento.SetAuditoriaModificacion(DateTime.UtcNow, request.UsuarioId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
