using MediatR;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.Entities;

namespace GalponERP.Application.Inventario.Queries.VerificarNivelesAlimento;

public record AlertaInventarioDto(
    decimal StockActualAlimento,
    decimal ConsumoDiarioGlobal,
    decimal DiasRestantes,
    bool RequiereAlerta);

public record VerificarNivelesAlimentoQuery() : IRequest<AlertaInventarioDto>;

public class VerificarNivelesAlimentoHandler : IRequestHandler<VerificarNivelesAlimentoQuery, AlertaInventarioDto>
{
    private readonly IInventarioRepository _inventarioRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IProductoRepository _productoRepository;

    public VerificarNivelesAlimentoHandler(
        IInventarioRepository inventarioRepository,
        ILoteRepository loteRepository,
        IProductoRepository productoRepository)
    {
        _inventarioRepository = inventarioRepository;
        _loteRepository = loteRepository;
        _productoRepository = productoRepository;
    }

    public async Task<AlertaInventarioDto> Handle(VerificarNivelesAlimentoQuery request, CancellationToken cancellationToken)
    {
        var todosLosProductos = await _productoRepository.ObtenerTodosAsync();
        var productosAlimento = todosLosProductos
            .Where(p => p.Categoria?.Tipo == TipoCategoria.Alimento)
            .ToDictionary(p => p.Id, p => p.PesoUnitarioKg);

        var alimentoIds = productosAlimento.Keys.ToHashSet();

        var todosLosMovimientos = await _inventarioRepository.ObtenerTodosAsync();
        var movimientosAlimento = todosLosMovimientos.Where(m => alimentoIds.Contains(m.ProductoId)).ToList();

        // Stock Total de Alimento en la granja en Kg (Incluye ajustes)
        var stockTotalKg = movimientosAlimento.Sum(m => 
        {
            decimal factor = (m.Tipo == TipoMovimiento.Entrada || m.Tipo == TipoMovimiento.AjusteEntrada) ? 1 : -1;
            return m.Cantidad * productosAlimento[m.ProductoId] * factor;
        });

        var lotesActivos = await _loteRepository.ObtenerActivosAsync();
        decimal consumoDiarioGlobalKg = 0;

        foreach (var lote in lotesActivos)
        {
            // El consumo para proyecciones solo debe considerar Salidas normales
            var movimientosLote = movimientosAlimento.Where(m => m.LoteId == lote.Id && m.Tipo == TipoMovimiento.Salida).ToList();
            
            if (!movimientosLote.Any()) continue;

            var diasDeVida = (DateTime.UtcNow - lote.FechaIngreso).TotalDays;
            if (diasDeVida < 1) diasDeVida = 1;

            var totalConsumidoLoteKg = movimientosLote.Sum(m => m.Cantidad * productosAlimento[m.ProductoId]);
            var consumoPromedioLoteKg = totalConsumidoLoteKg / (decimal)diasDeVida;

            consumoDiarioGlobalKg += consumoPromedioLoteKg;
        }

        decimal diasRestantes = consumoDiarioGlobalKg > 0 ? stockTotalKg / consumoDiarioGlobalKg : 999;
        bool requiereAlerta = diasRestantes < 3;

        return new AlertaInventarioDto(
            stockTotalKg,
            consumoDiarioGlobalKg,
            diasRestantes,
            requiereAlerta);
    }
}
