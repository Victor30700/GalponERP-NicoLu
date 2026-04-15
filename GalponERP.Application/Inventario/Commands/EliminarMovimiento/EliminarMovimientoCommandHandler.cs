using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Inventario.Commands.EliminarMovimiento;

public class EliminarMovimientoCommandHandler : IRequestHandler<EliminarMovimientoCommand>
{
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGalponDbContext _context;

    public EliminarMovimientoCommandHandler(
        IInventarioRepository inventarioRepository, 
        IUnitOfWork unitOfWork,
        IGalponDbContext context)
    {
        _inventarioRepository = inventarioRepository;
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

        // Usamos Desactivar (Soft Delete) del Entity base
        movimiento.Desactivar();
        movimiento.SetAuditoriaModificacion(DateTime.UtcNow, request.UsuarioId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
