using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Inventario.Queries.ObtenerReporteAjustes;

public class ObtenerReporteAjustesInventarioQueryHandler : IRequestHandler<ObtenerReporteAjustesInventarioQuery, IEnumerable<AjusteInventarioResponse>>
{
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;

    public ObtenerReporteAjustesInventarioQueryHandler(IInventarioRepository inventarioRepository, IProductoRepository productoRepository)
    {
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
    }

    public async Task<IEnumerable<AjusteInventarioResponse>> Handle(ObtenerReporteAjustesInventarioQuery request, CancellationToken cancellationToken)
    {
        var productos = (await _productoRepository.ObtenerTodosAsync()).ToDictionary(p => p.Id, p => p.Nombre);
        var movimientos = await _inventarioRepository.ObtenerTodosAsync();

        // Filtramos solo AjusteEntrada y AjusteSalida que tengan justificación
        var ajustes = movimientos
            .Where(m => (m.Tipo == TipoMovimiento.AjusteEntrada || m.Tipo == TipoMovimiento.AjusteSalida) && !string.IsNullOrWhiteSpace(m.Justificacion))
            .OrderByDescending(m => m.Fecha)
            .Select(m => new AjusteInventarioResponse(
                m.Id,
                m.ProductoId,
                productos.ContainsKey(m.ProductoId) ? productos[m.ProductoId] : "Producto desconocido",
                m.Cantidad,
                m.Tipo.ToString(),
                m.Fecha,
                m.Justificacion,
                m.LoteId,
                m.UsuarioId
            ));

        return ajustes;
    }
}
