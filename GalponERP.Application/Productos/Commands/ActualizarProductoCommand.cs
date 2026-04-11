using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Productos.Commands.ActualizarProducto;

public record ActualizarProductoCommand(
    Guid Id,
    string Nombre,
    TipoProducto Tipo,
    UnidadMedida UnidadMedida) : IRequest;

public class ActualizarProductoCommandHandler : IRequestHandler<ActualizarProductoCommand>
{
    private readonly IProductoRepository _productoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarProductoCommandHandler(IProductoRepository productoRepository, IUnitOfWork unitOfWork)
    {
        _productoRepository = productoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ActualizarProductoCommand request, CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.ObtenerPorIdAsync(request.Id);

        if (producto == null)
        {
            throw new Exception("Producto no encontrado.");
        }

        producto.Actualizar(request.Nombre, request.Tipo, request.UnidadMedida);

        _productoRepository.Actualizar(producto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
