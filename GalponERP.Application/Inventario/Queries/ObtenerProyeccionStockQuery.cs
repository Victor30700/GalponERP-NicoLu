using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Inventario.Queries.ObtenerProyeccionStock;

public record ObtenerProyeccionStockQuery() : IRequest<IEnumerable<ProyeccionStockResponse>>;

public record ProyeccionStockResponse(
    Guid ProductoId,
    string NombreProducto,
    decimal StockActual,
    decimal ConsumoDiarioEstimado,
    int DiasRestantes,
    DateTime FechaAgotamientoEstimada);

public class ObtenerProyeccionStockQueryHandler : IRequestHandler<ObtenerProyeccionStockQuery, IEnumerable<ProyeccionStockResponse>>
{
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly ILoteRepository _loteRepository;

    public ObtenerProyeccionStockQueryHandler(
        IInventarioRepository inventarioRepository, 
        IProductoRepository productoRepository,
        ILoteRepository loteRepository)
    {
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _loteRepository = loteRepository;
    }

    public async Task<IEnumerable<ProyeccionStockResponse>> Handle(ObtenerProyeccionStockQuery request, CancellationToken cancellationToken)
    {
        var lotesActivos = (await _loteRepository.ObtenerTodosAsync())
            .Where(l => l.Estado == EstadoLote.Activo)
            .ToList();

        var productos = await _productoRepository.ObtenerTodosAsync();
        var productosAlimento = productos
            .Where(p => p.Categoria?.Nombre.Equals("Alimento", StringComparison.OrdinalIgnoreCase) == true)
            .ToList();

        var proyecciones = new List<ProyeccionStockResponse>();

        foreach (var producto in productosAlimento)
        {
            decimal stockActual = await _inventarioRepository.ObtenerStockPorProductoIdAsync(producto.Id);
            decimal consumoDiarioTotalKg = 0;

            foreach (var lote in lotesActivos)
            {
                int edadDias = (DateTime.UtcNow - lote.FechaIngreso).Days + 1;
                decimal consumoPorPolloGramos = ObtenerConsumoEstimadoGramos(edadDias);
                
                // Consumo del lote en Kg
                decimal consumoLoteKg = (lote.CantidadActual * consumoPorPolloGramos) / 1000m;
                consumoDiarioTotalKg += consumoLoteKg;
            }

            // Convertir consumo diario de Kg a la unidad del producto
            decimal consumoDiarioEnUnidad = producto.EquivalenciaEnKg > 0 
                ? consumoDiarioTotalKg / producto.EquivalenciaEnKg 
                : 0;

            if (consumoDiarioEnUnidad > 0)
            {
                int diasRestantes = (int)Math.Floor(stockActual / consumoDiarioEnUnidad);
                proyecciones.Add(new ProyeccionStockResponse(
                    producto.Id,
                    producto.Nombre,
                    stockActual,
                    Math.Round(consumoDiarioEnUnidad, 2),
                    diasRestantes,
                    DateTime.UtcNow.AddDays(diasRestantes)
                ));
            }
        }

        return proyecciones;
    }

    private decimal ObtenerConsumoEstimadoGramos(int dia)
    {
        // Modelo simplificado de consumo diario acumulado (gramos por pollo por día)
        // Fuente: Guía de manejo Cobb/Ross (Aproximado)
        if (dia <= 7) return 20;   // Semana 1: ~20g/día
        if (dia <= 14) return 50;  // Semana 2: ~50g/día
        if (dia <= 21) return 90;  // Semana 3: ~90g/día
        if (dia <= 28) return 130; // Semana 4: ~130g/día
        if (dia <= 35) return 170; // Semana 5: ~170g/día
        return 200;                // Semana 6+: ~200g/día
    }
}
