using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Dashboard.Queries.ObtenerProyeccionSacrificio;

public class ObtenerProyeccionSacrificioQueryHandler : IRequestHandler<ObtenerProyeccionSacrificioQuery, ProyeccionSacrificioResponse?>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IPesajeLoteRepository _pesajeRepository;
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;

    private const decimal PESO_OBJETIVO = 2500m; // 2.5 Kg
    private const decimal GANANCIA_DIARIA_BASE = 50m; // 50g por día promedio
    private const decimal FCR_IDEAL = 1.6m;

    public ObtenerProyeccionSacrificioQueryHandler(
        ILoteRepository loteRepository,
        IPesajeLoteRepository pesajeRepository,
        IInventarioRepository inventarioRepository,
        IProductoRepository productoRepository)
    {
        _loteRepository = loteRepository;
        _pesajeRepository = pesajeRepository;
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
    }

    public async Task<ProyeccionSacrificioResponse?> Handle(ObtenerProyeccionSacrificioQuery request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null) return null;

        var pesajes = await _pesajeRepository.ObtenerPorLoteIdAsync(request.LoteId);
        var ultimoPesaje = pesajes.OrderByDescending(p => p.Fecha).FirstOrDefault();
        
        if (ultimoPesaje == null) return null;

        var movimientos = await _inventarioRepository.ObtenerPorLoteIdAsync(request.LoteId);
        var productos = await _productoRepository.ObtenerTodosAsync();
        var alimentoIds = productos.Where(p => p.Tipo == TipoProducto.Alimento).Select(p => p.Id);
        
        var alimentoConsumidoKg = movimientos
            .Where(m => alimentoIds.Contains(m.ProductoId) && m.Tipo == TipoMovimiento.Salida)
            .Sum(m => m.Cantidad);

        decimal fcrActual = 0;
        decimal biomasaGanadaKg = ((ultimoPesaje.PesoPromedioGramos - 40) / 1000) * lote.CantidadActual;
        
        if (biomasaGanadaKg > 0)
        {
            fcrActual = alimentoConsumidoKg / biomasaGanadaKg;
        }

        // Ajustar ganancia diaria según eficiencia (FCR)
        // Si el FCR es mayor al ideal, el crecimiento es más lento.
        decimal factorEficiencia = fcrActual > 0 ? FCR_IDEAL / fcrActual : 1.0m;
        decimal gananciaEstimada = GANANCIA_DIARIA_BASE * factorEficiencia;

        decimal faltanteGramos = PESO_OBJETIVO - ultimoPesaje.PesoPromedioGramos;
        int diasRestantes = (int)Math.Ceiling(faltanteGramos / (gananciaEstimada > 0 ? gananciaEstimada : 1));
        
        if (diasRestantes < 0) diasRestantes = 0;

        int diasDeVida = (DateTime.Now - lote.FechaIngreso).Days;

        return new ProyeccionSacrificioResponse(
            lote.Id,
            ultimoPesaje.PesoPromedioGramos,
            Math.Round(fcrActual, 2),
            diasDeVida,
            Math.Round(gananciaEstimada, 2),
            PESO_OBJETIVO,
            diasRestantes,
            DateTime.Now.AddDays(diasRestantes));
    }
}
