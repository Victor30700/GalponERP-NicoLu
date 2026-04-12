using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Proveedores.Queries.ListarProveedores;

public record ListarProveedoresQuery() : IRequest<IEnumerable<ProveedorResponse>>;

public record ProveedorResponse(
    Guid Id,
    string RazonSocial,
    string NitRuc,
    string? Telefono,
    string? Email,
    string? Direccion,
    bool IsActive);

public class ListarProveedoresQueryHandler : IRequestHandler<ListarProveedoresQuery, IEnumerable<ProveedorResponse>>
{
    private readonly IProveedorRepository _proveedorRepository;

    public ListarProveedoresQueryHandler(IProveedorRepository proveedorRepository)
    {
        _proveedorRepository = proveedorRepository;
    }

    public async Task<IEnumerable<ProveedorResponse>> Handle(ListarProveedoresQuery request, CancellationToken cancellationToken)
    {
        var proveedores = await _proveedorRepository.ObtenerTodosAsync();
        
        return proveedores.Select(p => new ProveedorResponse(
            p.Id,
            p.RazonSocial,
            p.NitRuc,
            p.Telefono,
            p.Email,
            p.Direccion,
            p.IsActive));
    }
}
