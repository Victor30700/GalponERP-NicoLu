using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Inventario.Commands.RegistrarConciliacion;

public class RegistrarConciliacionStockCommandHandler : IRequestHandler<RegistrarConciliacionStockCommand>
{
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarConciliacionStockCommandHandler(
        IInventarioRepository inventarioRepository,
        IProductoRepository productoRepository,
        IUnitOfWork unitOfWork)
    {
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RegistrarConciliacionStockCommand request, CancellationToken cancellationToken)
    {
        var movimientos = await _inventarioRepository.ObtenerTodosAsync();
        var stockPorProducto = movimientos
            .GroupBy(m => m.ProductoId)
            .Select(g => new
            {
                ProductoId = g.Key,
                Stock = g.Sum(m => (m.Tipo == TipoMovimiento.Entrada || 
                                   m.Tipo == TipoMovimiento.Compra || 
                                   m.Tipo == TipoMovimiento.AjusteEntrada) 
                                  ? m.Cantidad : -m.Cantidad)
            })
            .ToDictionary(x => x.ProductoId, x => x.Stock);

        foreach (var item in request.Items)
        {
            decimal stockSistema = stockPorProducto.ContainsKey(item.ProductoId) ? stockPorProducto[item.ProductoId] : 0;
            decimal diferencia = item.CantidadFisica - stockSistema;

            if (diferencia == 0) continue;

            var producto = await _productoRepository.ObtenerPorIdAsync(item.ProductoId);
            if (producto == null) continue;

            var tipoAjuste = diferencia > 0 ? TipoMovimiento.AjusteEntrada : TipoMovimiento.AjusteSalida;
            decimal cantidadAjuste = Math.Abs(diferencia);
            decimal costoTotalAjuste = cantidadAjuste * producto.CostoUnitarioActual;

            var movimiento = new MovimientoInventario(
                Guid.NewGuid(),
                item.ProductoId,
                null,
                cantidadAjuste,
                tipoAjuste,
                DateTime.UtcNow,
                request.UsuarioId,
                item.Nota ?? "Conciliación de inventario físico",
                new Moneda(costoTotalAjuste)
            );

            _inventarioRepository.RegistrarMovimiento(movimiento);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
