using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Galpones.Queries.ListarGalpones;

public class ListarGalponesQueryHandler : IRequestHandler<ListarGalponesQuery, IEnumerable<GalponResponse>>
{
    private readonly IGalponRepository _galponRepository;

    public ListarGalponesQueryHandler(IGalponRepository galponRepository)
    {
        _galponRepository = galponRepository;
    }

    public async Task<IEnumerable<GalponResponse>> Handle(ListarGalponesQuery request, CancellationToken cancellationToken)
    {
        var galpones = await _galponRepository.ObtenerTodosAsync();

        return galpones.Select(g => new GalponResponse(
            g.Id,
            g.Nombre,
            g.Capacidad,
            g.Ubicacion,
            g.IsActive));
    }
}
