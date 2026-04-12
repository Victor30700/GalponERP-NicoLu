using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Galpones.Queries.ObtenerGalponPorId;

public class ObtenerGalponPorIdQueryHandler : IRequestHandler<ObtenerGalponPorIdQuery, GalponDetalleResponse?>
{
    private readonly IGalponRepository _galponRepository;

    public ObtenerGalponPorIdQueryHandler(IGalponRepository galponRepository)
    {
        _galponRepository = galponRepository;
    }

    public async Task<GalponDetalleResponse?> Handle(ObtenerGalponPorIdQuery request, CancellationToken cancellationToken)
    {
        var galpon = await _galponRepository.ObtenerPorIdAsync(request.Id);

        if (galpon == null)
        {
            return null;
        }

        return new GalponDetalleResponse(
            galpon.Id,
            galpon.Nombre,
            galpon.Capacidad,
            galpon.Ubicacion,
            galpon.IsActive);
    }
}
