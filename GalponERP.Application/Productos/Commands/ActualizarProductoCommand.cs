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
    decimal PesoUnitarioKg = 0,
    decimal UmbralMinimo = 0,
    decimal StockInicial = 0) : IRequest;

public class ActualizarProductoCommandValidator : AbstractValidator<ActualizarProductoCommand>
{
    private readonly ICategoriaProductoRepository _categoriaRepository;
    private readonly IUnidadMedidaRepository _unidadMedidaRepository;

    public ActualizarProductoCommandValidator(ICategoriaProductoRepository categoriaRepository, IUnidadMedidaRepository unidadMedidaRepository)
    {
        _categoriaRepository = categoriaRepository;
        _unidadMedidaRepository = unidadMedidaRepository;

        RuleFor(x => x.Id).NotEmpty().WithMessage("El ID es obligatorio.");
        
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre no puede exceder los 200 caracteres.");
            
        RuleFor(x => x.CategoriaProductoId)
            .NotEmpty().WithMessage("La categoría es obligatoria.")
            .MustAsync(async (id, cancellation) => {
                var categoria = await _categoriaRepository.ObtenerPorIdAsync(id);
                return categoria != null;
            }).WithMessage("La categoría seleccionada no existe.");
            
        RuleFor(x => x.UnidadMedidaId)
            .NotEmpty().WithMessage("La unidad de medida es obligatoria.")
            .MustAsync(async (id, cancellation) => {
                var unidad = await _unidadMedidaRepository.ObtenerPorIdAsync(id);
                return unidad != null;
            }).WithMessage("La unidad de medida seleccionada no existe.");
            
        RuleFor(x => x.PesoUnitarioKg)
            .GreaterThanOrEqualTo(0).WithMessage("El peso unitario en Kg no puede ser negativo.");

        // Regla Condicional: Solo si la categoría es 'Alimento' se exige PesoUnitarioKg > 0
        RuleFor(x => x.PesoUnitarioKg)
            .CustomAsync(async (peso, context, cancellation) => {
                var categoriaId = context.InstanceToValidate.CategoriaProductoId;
                if (categoriaId == Guid.Empty) return;

                var categoria = await _categoriaRepository.ObtenerPorIdAsync(categoriaId);
                if (categoria != null && categoria.Nombre.Equals("Alimento", StringComparison.OrdinalIgnoreCase))
                {
                    if (peso <= 0)
                    {
                        context.AddFailure("Para productos de tipo 'Alimento', el peso por unidad (Kg) debe ser mayor a 0 para el correcto cálculo del FCR.");
                    }
                }
            });

        RuleFor(x => x.UmbralMinimo)
            .GreaterThanOrEqualTo(0).WithMessage("El umbral mínimo no puede ser negativo.");

        RuleFor(x => x.StockInicial)
            .GreaterThanOrEqualTo(0).WithMessage("El stock inicial no puede ser negativo.");
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
            request.UmbralMinimo,
            request.StockInicial);

        _productoRepository.Actualizar(producto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
