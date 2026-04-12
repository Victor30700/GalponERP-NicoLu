using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Pesajes.Queries.ObtenerPesajePorId;

public class ObtenerPesajePorIdQueryHandler : IRequestHandler<ObtenerPesajePorIdQuery, PesajeResponse?>
{
    private readonly IPesajeLoteRepository _pesajeRepository;

    public ObtenerPesajePorIdQueryHandler(IPesajeLoteRepository pesajeRepository)
    {
        _pesajeRepository = pesajeRepository;
    }

    public async Task<PesajeResponse?> Handle(ObtenerPesajePorIdQuery request, CancellationToken cancellationToken)
    {
        var pesaje = await _pesajeRepository.ObtenerPorIdAsync(request.Id);

        if (pesaje == null)
        {
            return null;
        }

        return new PesajeResponse(
            pesaje.Id,
            pesaje.LoteId,
            pesaje.Fecha,
            pesaje.PesoPromedioGramos,
            pesaje.CantidadMuestreada,
            pesaje.UsuarioId);
    }
}
