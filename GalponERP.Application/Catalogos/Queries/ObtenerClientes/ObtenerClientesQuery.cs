using MediatR;

namespace GalponERP.Application.Catalogos.Queries.ObtenerClientes;

public record ObtenerClientesQuery() : IRequest<IEnumerable<ClienteResponse>>;

public record ClienteResponse(
    Guid Id,
    string Nombre,
    string Ruc,
    string? Direccion,
    string? Telefono);
