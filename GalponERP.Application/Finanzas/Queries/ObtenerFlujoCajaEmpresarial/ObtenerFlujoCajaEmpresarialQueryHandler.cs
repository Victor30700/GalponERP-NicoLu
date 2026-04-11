using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Finanzas.Queries.ObtenerFlujoCajaEmpresarial;

public class ObtenerFlujoCajaEmpresarialQueryHandler : IRequestHandler<ObtenerFlujoCajaEmpresarialQuery, FlujoCajaDto>
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IGastoOperativoRepository _gastoRepository;
    private readonly ILoteRepository _loteRepository;

    public ObtenerFlujoCajaEmpresarialQueryHandler(
        IVentaRepository ventaRepository,
        IGastoOperativoRepository gastoRepository,
        ILoteRepository loteRepository)
    {
        _ventaRepository = ventaRepository;
        _gastoRepository = gastoRepository;
        _loteRepository = loteRepository;
    }

    public async Task<FlujoCajaDto> Handle(ObtenerFlujoCajaEmpresarialQuery request, CancellationToken cancellationToken)
    {
        var ventas = await _ventaRepository.ObtenerPorRangoFechasAsync(request.Inicio, request.Fin);
        var gastos = await _gastoRepository.ObtenerPorRangoFechasAsync(request.Inicio, request.Fin);
        var lotes = (await _loteRepository.ObtenerTodosAsync()).ToDictionary(l => l.Id, l => l.Id.ToString().Substring(0, 8)); // Simplificado para el DTO

        var ventasDto = ventas.Select(v => new VentaResumenDto(
            v.Id, 
            v.Fecha, 
            lotes.ContainsKey(v.LoteId) ? lotes[v.LoteId] : "N/A", 
            v.Total.Monto)).ToList();

        var gastosDto = gastos.Select(g => new GastoResumenDto(
            g.Id, 
            g.Fecha, 
            g.Descripcion, 
            g.TipoGasto, 
            g.Monto.Monto)).ToList();

        decimal totalIngresos = ventasDto.Sum(v => v.Monto);
        decimal totalEgresos = gastosDto.Sum(g => g.Monto);

        return new FlujoCajaDto(
            totalIngresos,
            totalEgresos,
            totalIngresos - totalEgresos,
            ventasDto,
            gastosDto
        );
    }
}
