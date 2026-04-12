using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Inventario.Queries.ObtenerKardexProducto;

public class ObtenerKardexProductoQueryHandler : IRequestHandler<ObtenerKardexProductoQuery, IEnumerable<KardexMovimientoResponse>>
{
    private readonly IInventarioRepository _inventarioRepository;

    public ObtenerKardexProductoQueryHandler(IInventarioRepository inventarioRepository)
    {
        _inventarioRepository = inventarioRepository;
    }

    public async Task<IEnumerable<KardexMovimientoResponse>> Handle(ObtenerKardexProductoQuery request, CancellationToken cancellationToken)
    {
        var movimientos = await _inventarioRepository.ObtenerPorProductoIdAsync(request.ProductoId);
        
        // Ordenar cronológicamente (más antiguo al más nuevo)
        var movimientosOrdenados = movimientos.OrderBy(m => m.Fecha).ToList();

        var resultado = new List<KardexMovimientoResponse>();
        decimal saldoAcumulado = 0;

        foreach (var m in movimientosOrdenados)
        {
            decimal impacto = (m.Tipo == TipoMovimiento.Entrada || 
                               m.Tipo == TipoMovimiento.AjusteEntrada || 
                               m.Tipo == TipoMovimiento.Compra) 
                              ? m.Cantidad : -m.Cantidad;
            
            saldoAcumulado += impacto;

            resultado.Add(new KardexMovimientoResponse(
                m.Fecha,
                m.Tipo.ToString(),
                m.Cantidad,
                saldoAcumulado,
                m.Justificacion,
                m.LoteId));
        }

        return resultado;
    }
}
