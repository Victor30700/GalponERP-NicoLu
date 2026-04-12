using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Proveedores.Commands.CrearProveedor;

public record CrearProveedorCommand(
    string RazonSocial,
    string NitRuc,
    string? Telefono,
    string? Email,
    string? Direccion) : IRequest<Guid>;

public class CrearProveedorCommandHandler : IRequestHandler<CrearProveedorCommand, Guid>
{
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearProveedorCommandHandler(IProveedorRepository proveedorRepository, IUnitOfWork unitOfWork)
    {
        _proveedorRepository = proveedorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CrearProveedorCommand request, CancellationToken cancellationToken)
    {
        var proveedor = new Proveedor(
            Guid.NewGuid(),
            request.RazonSocial,
            request.NitRuc,
            request.Telefono,
            request.Email,
            request.Direccion);

        _proveedorRepository.Agregar(proveedor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return proveedor.Id;
    }
}
