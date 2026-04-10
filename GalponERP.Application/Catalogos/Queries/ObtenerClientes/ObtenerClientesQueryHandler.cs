using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Catalogos.Queries.ObtenerClientes;

public class ObtenerClientesQueryHandler : IRequestHandler<ObtenerClientesQuery, IEnumerable<ClienteResponse>>
{
    private readonly IClienteRepository _clienteRepository;

    public ObtenerClientesQueryHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<IEnumerable<ClienteResponse>> Handle(ObtenerClientesQuery request, CancellationToken cancellationToken)
    {
        var clientes = await _clienteRepository.ObtenerTodosAsync();
        
        return clientes.Select(c => new ClienteResponse(
            c.Id,
            c.Nombre,
            c.Ruc,
            c.Direccion,
            c.Telefono));
    }
}
