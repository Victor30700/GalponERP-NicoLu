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
    decimal PesoUnitarioKg = 0,
    decimal UmbralMinimo = 0,
    decimal StockInicial = 0,
    int PeriodoRetiroDias = 0) : IRequest<Guid>;

public class CrearProductoCommandValidator : AbstractValidator<CrearProductoCommand>
{
    private readonly ICategoriaProductoRepository _categoriaRepository;
    private readonly IUnidadMedidaRepository _unidadMedidaRepository;

    public CrearProductoCommandValidator(ICategoriaProductoRepository categoriaRepository, IUnidadMedidaRepository unidadMedidaRepository)
    {
        _categoriaRepository = categoriaRepository;
        _unidadMedidaRepository = unidadMedidaRepository;

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

public class CrearProductoCommandHandler : IRequestHandler<CrearProductoCommand, Guid>
{
    private readonly IProductoRepository _productoRepository;
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUserContext;

    public CrearProductoCommandHandler(
        IProductoRepository productoRepository, 
        IInventarioRepository inventarioRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserContext currentUserContext)
    {
        _productoRepository = productoRepository;
        _inventarioRepository = inventarioRepository;
        _unitOfWork = unitOfWork;
        _currentUserContext = currentUserContext;
    }

    public async Task<Guid> Handle(CrearProductoCommand request, CancellationToken cancellationToken)
    {
        var producto = new Producto(
            Guid.NewGuid(),
            request.Nombre,
            request.CategoriaProductoId,
            request.UnidadMedidaId,
            request.PesoUnitarioKg,
            request.UmbralMinimo,
            0, // Costo inicial
            request.StockInicial,
            request.PeriodoRetiroDias);

        _productoRepository.Agregar(producto);

        if (request.StockInicial > 0)
        {
            var movimiento = new MovimientoInventario(
                Guid.NewGuid(),
                producto.Id,
                null, // No asociado a lote al crear el producto
                request.StockInicial,
                TipoMovimiento.Entrada,
                DateTime.UtcNow,
                _currentUserContext.UsuarioId ?? Guid.Empty,
                producto.PesoUnitarioKg, // Capturamos el peso actual como HISTÓRICO
                $"Carga inicial de stock al crear producto ({request.StockInicial} unidades)"
            );
            _inventarioRepository.RegistrarMovimiento(movimiento);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return producto.Id;
    }
}
