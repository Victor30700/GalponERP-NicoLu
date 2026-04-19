using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Sanidad.Queries.ObtenerUltimoRegistroBienestar;

public record ObtenerUltimoRegistroBienestarQuery(Guid LoteId) : IRequest<BienestarResponse?>;

public record BienestarResponse(
    Guid Id,
    Guid LoteId,
    DateTime Fecha,
    decimal? Temperatura,
    decimal? Humedad,
    decimal? ConsumoAgua,
    decimal? Ph,
    decimal? CloroPpm,
    string? Observaciones);

public class ObtenerUltimoRegistroBienestarQueryHandler : IRequestHandler<ObtenerUltimoRegistroBienestarQuery, BienestarResponse?>
{
    private readonly IRegistroBienestarRepository _repository;

    public ObtenerUltimoRegistroBienestarQueryHandler(IRegistroBienestarRepository repository)
    {
        _repository = repository;
    }

    public async Task<BienestarResponse?> Handle(ObtenerUltimoRegistroBienestarQuery request, CancellationToken cancellationToken)
    {
        var registros = await _repository.ObtenerPorLoteAsync(request.LoteId);
        var ultimo = registros.FirstOrDefault();

        if (ultimo == null) return null;

        return new BienestarResponse(
            ultimo.Id,
            ultimo.LoteId,
            ultimo.Fecha,
            ultimo.Temperatura,
            ultimo.Humedad,
            ultimo.ConsumoAgua,
            ultimo.Ph,
            ultimo.CloroPpm,
            ultimo.Observaciones);
    }
}
