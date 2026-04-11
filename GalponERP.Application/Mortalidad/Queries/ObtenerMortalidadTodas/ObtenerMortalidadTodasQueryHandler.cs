using GalponERP.Application.Mortalidad.Queries.ObtenerMortalidadPorLote;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Mortalidad.Queries.ObtenerMortalidadTodas;

public class ObtenerMortalidadTodasQueryHandler : IRequestHandler<ObtenerMortalidadTodasQuery, IEnumerable<MortalidadResponse>>
{
    private readonly IMortalidadRepository _mortalidadRepository;

    public ObtenerMortalidadTodasQueryHandler(IMortalidadRepository mortalidadRepository)
    {
        _mortalidadRepository = mortalidadRepository;
    }

    public async Task<IEnumerable<MortalidadResponse>> Handle(ObtenerMortalidadTodasQuery request, CancellationToken cancellationToken)
    {
        var mortalidad = await _mortalidadRepository.ObtenerTodasAsync();

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
