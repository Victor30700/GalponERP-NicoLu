using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Clientes.Queries.ListarClientes;

public record ListarClientesQuery() : IRequest<IEnumerable<ClienteResponse>>;

public record ClienteResponse(
    Guid Id,
    string Nombre,
    string Ruc,
    string? Direccion,
    string? Telefono,
    bool IsActive);

public class ListarClientesQueryHandler : IRequestHandler<ListarClientesQuery, IEnumerable<ClienteResponse>>
{
    private readonly IClienteRepository _clienteRepository;

    public ListarClientesQueryHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<IEnumerable<ClienteResponse>> Handle(ListarClientesQuery request, CancellationToken cancellationToken)
    {
        var clientes = await _clienteRepository.ObtenerTodosAsync();
        
        return clientes.Select(c => new ClienteResponse(
            c.Id,
            c.Nombre,
            c.Ruc,
            c.Direccion,
            c.Telefono,
            c.IsActive));
    }
}
