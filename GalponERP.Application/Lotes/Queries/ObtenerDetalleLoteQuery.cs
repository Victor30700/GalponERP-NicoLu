using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Lotes.Queries.ObtenerDetalleLote;

public record ObtenerDetalleLoteQuery(Guid LoteId) : IRequest<LoteDetalleResponse>;

public record LoteDetalleResponse(
    Guid Id,
    DateTime FechaIngreso,
    int CantidadInicial,
    int CantidadActual,
    int MortalidadTotal,
    int PollosVendidos,
    decimal CostoUnitarioPollito,
    string Estado,
    decimal TotalVentas,
    decimal TotalGastos,
    decimal UtilidadEstimada,
    IEnumerable<VentaItemResponse> Ventas,
    IEnumerable<MortalidadItemResponse> HistorialMortalidad,
    IEnumerable<GastoItemResponse> Gastos);

public record VentaItemResponse(Guid Id, DateTime Fecha, int Cantidad, decimal PrecioUnitario, decimal Total);
public record MortalidadItemResponse(Guid Id, DateTime Fecha, int Cantidad, string Causa);
public record GastoItemResponse(Guid Id, DateTime Fecha, string Descripcion, decimal Monto, string Tipo);

public class ObtenerDetalleLoteQueryHandler : IRequestHandler<ObtenerDetalleLoteQuery, LoteDetalleResponse>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IVentaRepository _ventaRepository;
    private readonly IMortalidadRepository _mortalidadRepository;
    private readonly IGastoOperativoRepository _gastoRepository;

    public ObtenerDetalleLoteQueryHandler(
        ILoteRepository loteRepository,
        IVentaRepository ventaRepository,
        IMortalidadRepository mortalidadRepository,
        IGastoOperativoRepository gastoRepository)
    {
        _loteRepository = loteRepository;
        _ventaRepository = ventaRepository;
        _mortalidadRepository = mortalidadRepository;
        _gastoRepository = gastoRepository;
    }

    public async Task<LoteDetalleResponse> Handle(ObtenerDetalleLoteQuery request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null) return null!;

        var ventas = await _ventaRepository.ObtenerPorLoteAsync(request.LoteId);
        var mortalidad = await _mortalidadRepository.ObtenerPorLoteAsync(request.LoteId);
        var gastos = await _gastoRepository.ObtenerPorLoteAsync(request.LoteId);

        var totalVentas = ventas.Sum(v => v.Total.Monto);
        var totalGastos = gastos.Sum(g => g.Monto.Monto);
        var costoPollitos = lote.CantidadInicial * lote.CostoUnitarioPollito.Monto;
        var utilidad = totalVentas - totalGastos - costoPollitos;

        return new LoteDetalleResponse(
            lote.Id,
            lote.FechaIngreso,
            lote.CantidadInicial,
            lote.CantidadActual,
            lote.MortalidadAcumulada,
            lote.PollosVendidos,
            lote.CostoUnitarioPollito.Monto,
            lote.Estado.ToString(),
            totalVentas,
            totalGastos,
            utilidad,
            ventas.Select(v => new VentaItemResponse(v.Id, v.Fecha, v.CantidadPollos, v.PrecioUnitario.Monto, v.Total.Monto)),
            mortalidad.Select(m => new MortalidadItemResponse(m.Id, m.Fecha, m.CantidadBajas, m.Causa)),
            gastos.Select(g => new GastoItemResponse(g.Id, g.Fecha, g.Descripcion, g.Monto.Monto, g.TipoGasto))
        );
    }
}
