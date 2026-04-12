using FluentValidation;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Productos.Commands.CrearProducto;

public record CrearProductoCommand(
    string Nombre,
    Guid CategoriaProductoId,
    Guid UnidadMedidaId,
    decimal EquivalenciaEnKg,
    decimal UmbralMinimo = 0) : IRequest<Guid>;

public class CrearProductoCommandValidator : AbstractValidator<CrearProductoCommand>
{
    public CrearProductoCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre no puede exceder los 200 caracteres.");
            
        RuleFor(x => x.CategoriaProductoId)
            .NotEmpty().WithMessage("La categoría es obligatoria.");
            
        RuleFor(x => x.UnidadMedidaId)
            .NotEmpty().WithMessage("La unidad de medida es obligatoria.");
            
        RuleFor(x => x.EquivalenciaEnKg)
            .GreaterThan(0).WithMessage("La equivalencia en Kg debe ser mayor a cero.");

        RuleFor(x => x.UmbralMinimo)
            .GreaterThanOrEqualTo(0).WithMessage("El umbral mínimo no puede ser negativo.");
    }
}

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
            request.CategoriaProductoId,
            request.UnidadMedidaId,
            request.EquivalenciaEnKg,
            request.UmbralMinimo);

        _productoRepository.Agregar(producto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return producto.Id;
    }
}
