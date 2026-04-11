using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Productos.Commands.CrearProducto;

public record CrearProductoCommand(
    string Nombre,
    TipoProducto Tipo,
    UnidadMedida UnidadMedida) : IRequest<Guid>;

public class CrearProductoCommandHandler : IRequestHandler<CrearProductoCommand, Guid>
{
    private readonly IProductoRepository _productoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearProductoCommandHandler(IProductoRepository productoRepository, IUnitOfWork unitOfWork)
    {
        _productoRepository = productoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CrearProductoCommand request, CancellationToken cancellationToken)
    {
        var producto = new Producto(
            Guid.NewGuid(),
            request.Nombre,
            request.Tipo,
            request.UnidadMedida);

        _productoRepository.Agregar(producto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return producto.Id;
    }
}
