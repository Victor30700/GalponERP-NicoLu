using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Lotes.Queries.ListarLotes;

public record ListarLotesQuery(
    bool SoloActivos = true,
    string? Busqueda = null,
    int? Mes = null,
    int? Anio = null) : IRequest<IEnumerable<LoteResponse>>;

public record LoteResponse(
    Guid Id,
    string Nombre,
    string NombreLote, // Compatibilidad
    Guid GalponId,
    string GalponNombre,
    string NombreGalpon, // Compatibilidad
    DateTime FechaInicio, // Compatibilidad
    DateTime FechaIngreso,
    int CantidadInicial,
    int CantidadActual,
    int AvesVivas, // Compatibilidad
    int MortalidadAcumulada,
    int PollosVendidos,
    decimal CostoUnitarioPollito,
    int EdadSemanas,
    string Estado,
    decimal FcrActual,
    decimal MortalidadPorcentaje,
    decimal Viabilidad,
    decimal CostoPorAve,
    string? Version);

public class ListarLotesQueryHandler : IRequestHandler<ListarLotesQuery, IEnumerable<LoteResponse>>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IPesajeLoteRepository _pesajeRepository;
    private readonly IGastoOperativoRepository _gastoRepository;
    private readonly ICategoriaProductoRepository _categoriaRepository;
    private readonly IProductoRepository _productoRepository;

    public ListarLotesQueryHandler(
        ILoteRepository loteRepository,
        IInventarioRepository inventarioRepository,
        IPesajeLoteRepository pesajeRepository,
        IGastoOperativoRepository gastoRepository,
        ICategoriaProductoRepository categoriaRepository,
        IProductoRepository productoRepository)
    {
        _loteRepository = loteRepository;
        _inventarioRepository = inventarioRepository;
        _pesajeRepository = pesajeRepository;
        _gastoRepository = gastoRepository;
        _categoriaRepository = categoriaRepository;
        _productoRepository = productoRepository;
    }

    public async Task<IEnumerable<LoteResponse>> Handle(ListarLotesQuery request, CancellationToken cancellationToken)
    {
        var lotes = await _loteRepository.ObtenerFiltradosAsync(request.Busqueda, request.Mes, request.Anio, request.SoloActivos);
        var loteList = lotes.ToList();
        var loteIds = loteList.Select(l => l.Id).ToList();

        if (!loteIds.Any()) return Enumerable.Empty<LoteResponse>();

        // 1. Fetch related data in bulk
        var allMovimientos = await _inventarioRepository.ObtenerPorVariosLotesAsync(loteIds);
        var allPesajes = await _pesajeRepository.ObtenerPorVariosLotesAsync(loteIds);
        var allGastos = await _gastoRepository.ObtenerPorVariosLotesAsync(loteIds);
        
        // 2. Identify Food Products for FCR and Consumption Costs
        var categoriasAlimento = (await _categoriaRepository.ObtenerTodasAsync())
            .Where(c => c.Tipo == TipoCategoria.Alimento)
            .Select(c => c.Id)
            .ToList();
            
        var productosAlimento = (await _productoRepository.ObtenerTodosAsync())
            .Where(p => categoriasAlimento.Contains(p.CategoriaProductoId))
            .Select(p => p.Id)
            .ToHashSet();

        return loteList.Select(l => {
            var movimientosLote = allMovimientos.Where(m => m.LoteId == l.Id).ToList();
            var pesajesLote = allPesajes.Where(p => p.LoteId == l.Id).ToList();
            var gastosLote = allGastos.Where(g => g.LoteId == l.Id).ToList();

            // Mortalidad y Viabilidad
            decimal mortalidadPorc = l.CantidadInicial > 0 ? Math.Round((decimal)l.MortalidadAcumulada / l.CantidadInicial * 100, 1) : 0;
            decimal viabilidad = 100 - mortalidadPorc;

            // FCR (ICA)
            var totalAlimentoKg = movimientosLote
                .Where(m => m.Tipo == TipoMovimiento.Salida && productosAlimento.Contains(m.ProductoId))
                .Sum(m => m.Cantidad * m.PesoUnitarioHistorico);

            var ultimoPesaje = pesajesLote.OrderByDescending(p => p.Fecha).FirstOrDefault();
            decimal fcr = 0;
            if (totalAlimentoKg > 0 && ultimoPesaje != null && ultimoPesaje.PesoPromedioGramos > 0)
            {
                decimal pesoKg = ultimoPesaje.PesoPromedioGramos / 1000m;
                fcr = l.CalcularFCRActual(totalAlimentoKg, pesoKg);
            }

            // Coste por Ave
            var totalGastosOp = gastosLote.Sum(g => g.Monto.Monto);
            var costoPollitos = l.CantidadInicial * l.CostoUnitarioPollito.Monto;
            
            // Aproximación del costo de alimento (si no está en el movimiento, asumimos un promedio si es posible, 
            // pero aquí sumaremos lo que haya en CostoTotal del movimiento si está disponible)
            var costoAlimento = movimientosLote
                .Where(m => m.Tipo == TipoMovimiento.Salida && productosAlimento.Contains(m.ProductoId))
                .Sum(m => m.CostoTotal?.Monto ?? 0);

            decimal costoTotalAcumulado = totalGastosOp + costoPollitos + costoAlimento;
            decimal costoPorAve = l.CantidadActual > 0 ? Math.Round(costoTotalAcumulado / l.CantidadActual, 2) : 0;

            return new LoteResponse(
                l.Id,
                l.Nombre,
                l.Nombre,
                l.GalponId,
                l.Galpon?.Nombre ?? "N/A",
                l.Galpon?.Nombre ?? "N/A",
                l.FechaIngreso,
                l.FechaIngreso,
                l.CantidadInicial,
                l.CantidadActual,
                l.CantidadActual,
                l.MortalidadAcumulada,
                l.PollosVendidos,
                l.CostoUnitarioPollito.Monto,
                l.EdadSemanas,
                l.Estado.ToString(),
                fcr,
                mortalidadPorc,
                viabilidad,
                costoPorAve,
                l.Version.ToString());
        });
    }
}
