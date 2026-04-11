using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Pesajes.Queries.ObtenerPesajesPorLote;

public class ObtenerPesajesPorLoteQueryHandler : IRequestHandler<ObtenerPesajesPorLoteQuery, IEnumerable<PesajeResponse>>
{
    private readonly IPesajeLoteRepository _pesajeRepository;

    public ObtenerPesajesPorLoteQueryHandler(IPesajeLoteRepository pesajeRepository)
    {
        _pesajeRepository = pesajeRepository;
    }

    public async Task<IEnumerable<PesajeResponse>> Handle(ObtenerPesajesPorLoteQuery request, CancellationToken cancellationToken)
    {
        var pesajes = await _pesajeRepository.ObtenerPorLoteIdAsync(request.LoteId);

        return pesajes.Select(p => new PesajeResponse(
            p.Id,
            p.LoteId,
            p.Fecha,
            p.PesoPromedioGramos,
            p.CantidadMuestreada));
    }
}
