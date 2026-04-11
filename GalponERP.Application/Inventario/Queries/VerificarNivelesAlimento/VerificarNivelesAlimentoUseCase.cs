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
        var alimentos = await _productoRepository.ObtenerPorTipoAsync(TipoProducto.Alimento);
        var alimentoIds = alimentos.Select(p => p.Id).ToHashSet();

        var todosLosMovimientos = await _inventarioRepository.ObtenerTodosAsync();
        var movimientosAlimento = todosLosMovimientos.Where(m => alimentoIds.Contains(m.ProductoId)).ToList();

        // Stock Total de Alimento en la granja (Incluye ajustes)
        var stockTotal = movimientosAlimento.Sum(m => 
            (m.Tipo == TipoMovimiento.Entrada || m.Tipo == TipoMovimiento.AjusteEntrada) ? m.Cantidad : -m.Cantidad);

        var lotesActivos = await _loteRepository.ObtenerActivosAsync();
        decimal consumoDiarioGlobal = 0;

        foreach (var lote in lotesActivos)
        {
            // El consumo para proyecciones solo debe considerar Salidas normales
            var movimientosLote = movimientosAlimento.Where(m => m.LoteId == lote.Id && m.Tipo == TipoMovimiento.Salida).ToList();
            
            if (!movimientosLote.Any()) continue;

            var diasDeVida = (DateTime.UtcNow - lote.FechaIngreso).TotalDays;
            if (diasDeVida < 1) diasDeVida = 1;

            var totalConsumidoLote = movimientosLote.Sum(m => m.Cantidad);
            var consumoPromedioLote = totalConsumidoLote / (decimal)diasDeVida;

            consumoDiarioGlobal += consumoPromedioLote;
        }

        decimal diasRestantes = consumoDiarioGlobal > 0 ? stockTotal / consumoDiarioGlobal : 999;
        bool requiereAlerta = diasRestantes < 3;

        return new AlertaInventarioDto(
            stockTotal,
            consumoDiarioGlobal,
            diasRestantes,
            requiereAlerta);
    }
}
