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
    decimal PesoUnitarioKg,
    decimal UmbralMinimo = 0,
    decimal StockInicial = 0,
    decimal? EquivalenciaEnKg = null) : IRequest<Guid>;

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
            
        RuleFor(x => x.PesoUnitarioKg)
            .GreaterThan(0).WithMessage("El peso unitario en Kg debe ser mayor a cero.");

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
        // El peso total inicial puede venir del DTO (como EquivalenciaEnKg según el usuario)
        // o calcularse como StockInicial * PesoUnitarioKg
        decimal stockInicialKg = request.EquivalenciaEnKg ?? (request.StockInicial * request.PesoUnitarioKg);

        var producto = new Producto(
            Guid.NewGuid(),
            request.Nombre,
            request.CategoriaProductoId,
            request.UnidadMedidaId,
            request.PesoUnitarioKg,
            request.UmbralMinimo,
            0, // Costo inicial
            stockInicialKg);

        _productoRepository.Agregar(producto);

        if (request.StockInicial > 0)
        {
            var movimiento = new MovimientoInventario(
                Guid.NewGuid(),
                producto.Id,
                null, // No asociado a lote
                request.StockInicial,
                TipoMovimiento.Entrada,
                DateTime.UtcNow,
                _currentUserContext.UsuarioId ?? Guid.Empty,
                "Carga inicial de stock al crear producto"
            );
            _inventarioRepository.RegistrarMovimiento(movimiento);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return producto.Id;
    }
}
