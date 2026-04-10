using MediatR;
using GalponERP.Domain.Services;

namespace GalponERP.Application.Planificacion.Queries.GetSimulacionRentabilidad;

public record GetSimulacionRentabilidadQuery(
    int CantidadPollos,
    decimal PesoEsperadoPorPolloKg,
    decimal PrecioAlimentoPorKg,
    decimal PrecioVentaPorKg,
    decimal? FcrPersonalizado = null) : IRequest<ResultadoSimulacion>;

public class GetSimulacionRentabilidadQueryHandler : IRequestHandler<GetSimulacionRentabilidadQuery, ResultadoSimulacion>
{
    private readonly SimuladorProyeccionLote _simulador;

    public GetSimulacionRentabilidadQueryHandler(SimuladorProyeccionLote simulador)
    {
        _simulador = simulador;
    }

    public Task<ResultadoSimulacion> Handle(GetSimulacionRentabilidadQuery request, CancellationToken cancellationToken)
    {
        var resultado = _simulador.Proyectar(
            request.CantidadPollos,
            request.PesoEsperadoPorPolloKg,
            request.PrecioAlimentoPorKg,
            request.PrecioVentaPorKg,
            request.FcrPersonalizado
        );

        return Task.FromResult(resultado);
    }
}
