using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Productos.Commands.EliminarProducto;

public record EliminarProductoCommand(Guid Id) : IRequest, IAuditableCommand;

public class EliminarProductoCommandHandler : IRequestHandler<EliminarProductoCommand>
{
    private readonly IProductoRepository _productoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarProductoCommandHandler(IProductoRepository productoRepository, IUnitOfWork unitOfWork)
    {
        _productoRepository = productoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EliminarProductoCommand request, CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.ObtenerPorIdAsync(request.Id);

        if (producto == null)
        {
            throw new Exception("Producto no encontrado.");
        }

        producto.Desactivar();

        _productoRepository.Actualizar(producto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
