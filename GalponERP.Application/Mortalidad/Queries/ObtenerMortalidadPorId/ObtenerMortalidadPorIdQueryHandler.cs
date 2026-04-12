using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Mortalidad.Queries.ObtenerMortalidadPorId;

public class ObtenerMortalidadPorIdQueryHandler : IRequestHandler<ObtenerMortalidadPorIdQuery, MortalidadResponse?>
{
    private readonly IMortalidadRepository _mortalidadRepository;

    public ObtenerMortalidadPorIdQueryHandler(IMortalidadRepository mortalidadRepository)
    {
        _mortalidadRepository = mortalidadRepository;
    }

    public async Task<MortalidadResponse?> Handle(ObtenerMortalidadPorIdQuery request, CancellationToken cancellationToken)
    {
        var mortalidad = await _mortalidadRepository.ObtenerPorIdAsync(request.Id);

        if (mortalidad == null)
        {
            return null;
        }

        return new MortalidadResponse(
            mortalidad.Id,
            mortalidad.LoteId,
            mortalidad.Fecha,
            mortalidad.CantidadBajas,
            mortalidad.Causa,
            mortalidad.UsuarioId);
    }
}
