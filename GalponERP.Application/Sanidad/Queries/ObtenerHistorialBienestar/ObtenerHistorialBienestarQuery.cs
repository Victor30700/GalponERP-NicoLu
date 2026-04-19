using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Sanidad.Queries.ObtenerHistorialBienestar;

public record ObtenerHistorialBienestarQuery(Guid LoteId) : IRequest<IEnumerable<BienestarHistorialResponse>>;

public record BienestarHistorialResponse(
    Guid Id,
    Guid LoteId,
    DateTime Fecha,
    decimal? Temperatura,
    decimal? Humedad,
    decimal? ConsumoAgua,
    decimal? Ph,
    decimal? CloroPpm,
    string? Observaciones);

public class ObtenerHistorialBienestarQueryHandler : IRequestHandler<ObtenerHistorialBienestarQuery, IEnumerable<BienestarHistorialResponse>>
{
    private readonly IRegistroBienestarRepository _repository;

    public ObtenerHistorialBienestarQueryHandler(IRegistroBienestarRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BienestarHistorialResponse>> Handle(ObtenerHistorialBienestarQuery request, CancellationToken cancellationToken)
    {
        var registros = await _repository.ObtenerPorLoteAsync(request.LoteId);
        
        return registros.OrderByDescending(r => r.Fecha).Select(r => new BienestarHistorialResponse(
            r.Id,
            r.LoteId,
            r.Fecha,
            r.Temperatura,
            r.Humedad,
            r.ConsumoAgua,
            r.Ph,
            r.CloroPpm,
            r.Observaciones));
    }
}
