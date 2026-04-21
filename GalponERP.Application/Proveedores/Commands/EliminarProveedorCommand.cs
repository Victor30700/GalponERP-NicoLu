using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Proveedores.Commands.EliminarProveedor;

public record EliminarProveedorCommand(Guid Id) : IRequest, IAuditableCommand;

public class EliminarProveedorCommandHandler : IRequestHandler<EliminarProveedorCommand>
{
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarProveedorCommandHandler(IProveedorRepository proveedorRepository, IUnitOfWork unitOfWork)
    {
        _proveedorRepository = proveedorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EliminarProveedorCommand request, CancellationToken cancellationToken)
    {
        var proveedor = await _proveedorRepository.ObtenerPorIdAsync(request.Id);

        if (proveedor == null)
        {
            throw new Exception($"Proveedor con ID {request.Id} no encontrado.");
        }

        proveedor.Eliminar();
        _proveedorRepository.Actualizar(proveedor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
