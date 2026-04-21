using GalponERP.Application.Exceptions;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Proveedores.Commands.ActualizarProveedor;

public record ActualizarProveedorCommand(
    Guid Id,
    string RazonSocial,
    string NitRuc,
    string? Telefono,
    string? Email,
    string? Direccion,
    string? Version = null) : IRequest, IAuditableCommand;

public class ActualizarProveedorCommandHandler : IRequestHandler<ActualizarProveedorCommand>
{
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarProveedorCommandHandler(IProveedorRepository proveedorRepository, IUnitOfWork unitOfWork)
    {
        _proveedorRepository = proveedorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ActualizarProveedorCommand request, CancellationToken cancellationToken)
    {
        var proveedor = await _proveedorRepository.ObtenerPorIdAsync(request.Id);

        if (proveedor == null)
        {
            throw new Exception($"Proveedor con ID {request.Id} no encontrado.");
        }

        if (!string.IsNullOrEmpty(request.Version) && proveedor.Version.ToString() != request.Version)
        {
            throw new ConcurrencyException();
        }

        proveedor.Actualizar(
            request.RazonSocial,
            request.NitRuc,
            request.Telefono,
            request.Email,
            request.Direccion);

        _proveedorRepository.Actualizar(proveedor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
