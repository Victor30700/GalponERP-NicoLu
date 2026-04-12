using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Lotes.Queries.ObtenerRendimientoVivo;

public class ObtenerRendimientoVivoLoteQueryHandler : IRequestHandler<ObtenerRendimientoVivoLoteQuery, RendimientoVivoResponse>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IPesajeLoteRepository _pesajeRepository;
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IGastoOperativoRepository _gastoRepository;

    public ObtenerRendimientoVivoLoteQueryHandler(
        ILoteRepository loteRepository,
        IPesajeLoteRepository pesajeRepository,
        IInventarioRepository inventarioRepository,
        IProductoRepository productoRepository,
        IGastoOperativoRepository gastoRepository)
    {
        _loteRepository = loteRepository;
        _pesajeRepository = pesajeRepository;
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _gastoRepository = gastoRepository;
    }

    public async Task<RendimientoVivoResponse> Handle(ObtenerRendimientoVivoLoteQuery request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null) return null!;

        var pesajes = await _pesajeRepository.ObtenerPorLoteIdAsync(request.LoteId);
        var ultimoPesaje = pesajes.OrderByDescending(p => p.Fecha).FirstOrDefault();
        decimal pesoPromedioActualGramos = ultimoPesaje?.PesoPromedioGramos ?? 0;

        var movimientos = await _inventarioRepository.ObtenerPorLoteIdAsync(request.LoteId);
        var productos = await _productoRepository.ObtenerTodosAsync();
        var gastos = await _gastoRepository.ObtenerPorLoteAsync(request.LoteId);

        var productosAlimento = productos
            .Where(p => p.Categoria?.Nombre.Equals("Alimento", StringComparison.OrdinalIgnoreCase) == true)
            .ToDictionary(p => p.Id, p => p.EquivalenciaEnKg);

        decimal alimentoConsumidoKg = movimientos
            .Where(m => (m.Tipo == TipoMovimiento.Salida || m.Tipo == TipoMovimiento.AjusteSalida) && productosAlimento.ContainsKey(m.ProductoId))
            .Sum(m => m.Cantidad * productosAlimento[m.ProductoId]);

        var costoAlimentoAcumulado = movimientos
            .Where(m => m.Tipo == TipoMovimiento.Salida || m.Tipo == TipoMovimiento.AjusteSalida)
            .Select(m => m.CostoTotal ?? Moneda.Zero)
            .Aggregate(Moneda.Zero, (acc, next) => acc + next);

        var costoPollitos = lote.CostoUnitarioPollito * lote.CantidadInicial;
        var gastosOperativosAcumulados = gastos.Select(g => g.Monto).Aggregate(Moneda.Zero, (acc, next) => acc + next);
        var costoTotalInvertido = costoPollitos + costoAlimentoAcumulado + gastosOperativosAcumulados;

        decimal biomasaTotalKg = (pesoPromedioActualGramos / 1000) * lote.CantidadActual;
        
        decimal fcrProyectado = 0;
        if (biomasaTotalKg > 0)
        {
            // FCR = Alimento Consumido / Biomasa Actual (Simplificado para rendimiento en vivo)
            fcrProyectado = Math.Round(alimentoConsumidoKg / biomasaTotalKg, 2);
        }

        decimal costoPorKiloVivo = 0;
        if (biomasaTotalKg > 0)
        {
            costoPorKiloVivo = Math.Round(costoTotalInvertido.Monto / biomasaTotalKg, 2);
        }

        int diasDeVida = (int)(DateTime.UtcNow - lote.FechaIngreso).TotalDays;

        return new RendimientoVivoResponse(
            lote.Id,
            diasDeVida,
            pesoPromedioActualGramos,
            biomasaTotalKg,
            alimentoConsumidoKg,
            fcrProyectado,
            costoAlimentoAcumulado.Monto,
            costoPollitos.Monto,
            gastosOperativosAcumulados.Monto,
            costoTotalInvertido.Monto,
            costoPorKiloVivo
        );
    }
}
