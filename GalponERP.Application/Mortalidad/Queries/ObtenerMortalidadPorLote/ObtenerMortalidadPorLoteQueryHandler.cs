using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Mortalidad.Queries.ObtenerMortalidadPorLote;

public class ObtenerMortalidadPorLoteQueryHandler : IRequestHandler<ObtenerMortalidadPorLoteQuery, IEnumerable<MortalidadResponse>>
{
    private readonly IMortalidadRepository _mortalidadRepository;

    public ObtenerMortalidadPorLoteQueryHandler(IMortalidadRepository mortalidadRepository)
    {
        _mortalidadRepository = mortalidadRepository;
    }

    public async Task<IEnumerable<MortalidadResponse>> Handle(ObtenerMortalidadPorLoteQuery request, CancellationToken cancellationToken)
    {
        var mortalidad = await _mortalidadRepository.ObtenerPorLoteAsync(request.LoteId);

        return mortalidad
            .OrderByDescending(m => m.Fecha)
            .Select(m => new MortalidadResponse(
                m.Id,
                m.LoteId,
                m.Fecha,
                m.CantidadBajas,
                m.Causa));
    }
}
