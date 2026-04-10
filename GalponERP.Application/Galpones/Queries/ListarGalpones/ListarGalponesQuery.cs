using MediatR;

namespace GalponERP.Application.Galpones.Queries.ListarGalpones;

public record ListarGalponesQuery() : IRequest<IEnumerable<GalponResponse>>;

public record GalponResponse(
    Guid Id,
    string Nombre,
    int Capacidad,
    string Ubicacion,
    bool IsActive);
