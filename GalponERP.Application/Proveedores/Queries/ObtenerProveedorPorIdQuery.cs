using GalponERP.Application.Proveedores.Queries.ListarProveedores;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Proveedores.Queries.ObtenerProveedorPorId;

public record ObtenerProveedorPorIdQuery(Guid Id) : IRequest<ProveedorResponse?>;

public class ObtenerProveedorPorIdQueryHandler : IRequestHandler<ObtenerProveedorPorIdQuery, ProveedorResponse?>
{
    private readonly IProveedorRepository _proveedorRepository;

    public ObtenerProveedorPorIdQueryHandler(IProveedorRepository proveedorRepository)
    {
        _proveedorRepository = proveedorRepository;
    }

    public async Task<ProveedorResponse?> Handle(ObtenerProveedorPorIdQuery request, CancellationToken cancellationToken)
    {
        var p = await _proveedorRepository.ObtenerPorIdAsync(request.Id);
        
        if (p == null) return null;

        return new ProveedorResponse(
            p.Id,
            p.RazonSocial,
            p.NitRuc,
            p.Telefono,
            p.Email,
            p.Direccion,
            p.IsActive);
    }
}
