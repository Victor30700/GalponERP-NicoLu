using FluentValidation;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Productos.Commands.ActualizarProducto;

public record ActualizarProductoCommand(
    Guid Id,
    string Nombre,
    Guid CategoriaProductoId,
    Guid UnidadMedidaId,
    decimal PesoUnitarioKg,
    decimal UmbralMinimo) : IRequest;

public class ActualizarProductoCommandValidator : AbstractValidator<ActualizarProductoCommand>
{
    public ActualizarProductoCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El ID es obligatorio.");
        
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre no puede exceder los 200 caracteres.");
            
        RuleFor(x => x.CategoriaProductoId)
            .NotEmpty().WithMessage("La categoría es obligatoria.");
            
        RuleFor(x => x.UnidadMedidaId)
            .NotEmpty().WithMessage("La unidad de medida es obligatoria.");
            
        RuleFor(x => x.PesoUnitarioKg)
            .GreaterThan(0).WithMessage("El peso unitario en Kg debe ser mayor a cero.");

        RuleFor(x => x.UmbralMinimo)
            .GreaterThanOrEqualTo(0).WithMessage("El umbral mínimo no puede ser negativo.");
    }
}

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

        producto.Actualizar(
            request.Nombre, 
            request.CategoriaProductoId, 
            request.UnidadMedidaId, 
            request.PesoUnitarioKg,
            request.UmbralMinimo);

        _productoRepository.Actualizar(producto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
