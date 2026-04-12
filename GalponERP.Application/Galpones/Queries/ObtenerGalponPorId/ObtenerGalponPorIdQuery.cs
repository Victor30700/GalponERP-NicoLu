using MediatR;

namespace GalponERP.Application.Galpones.Queries.ObtenerGalponPorId;

public record ObtenerGalponPorIdQuery(Guid Id) : IRequest<GalponDetalleResponse?>;

public record GalponDetalleResponse(
    Guid Id,
    string Nombre,
    int Capacidad,
    string Ubicacion,
    bool IsActive);
