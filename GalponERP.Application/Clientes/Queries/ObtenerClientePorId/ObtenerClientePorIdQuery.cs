using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Clientes.Queries.ObtenerClientePorId;

public record ObtenerClientePorIdQuery(Guid Id) : IRequest<ClienteResponse?>;

public record ClienteResponse(
    Guid Id,
    string Nombre,
    string Ruc,
    string? Direccion,
    string? Telefono,
    bool IsActive);

public class ObtenerClientePorIdQueryHandler : IRequestHandler<ObtenerClientePorIdQuery, ClienteResponse?>
{
    private readonly IClienteRepository _clienteRepository;

    public ObtenerClientePorIdQueryHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ClienteResponse?> Handle(ObtenerClientePorIdQuery request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObtenerPorIdAsync(request.Id);
        
        if (cliente == null)
            return null;

        return new ClienteResponse(
            cliente.Id,
            cliente.Nombre,
            cliente.Ruc,
            cliente.Direccion,
            cliente.Telefono,
            cliente.IsActive);
    }
}
