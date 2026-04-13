using FluentValidation;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Inventario.Commands.CrearOrdenCompra;

public record OrdenCompraItemDto(Guid ProductoId, decimal Cantidad, decimal PrecioUnitario);

public record CrearOrdenCompraCommand(
    Guid ProveedorId,
    List<OrdenCompraItemDto> Items,
    string? Nota = null) : IRequest<Guid>;

public class CrearOrdenCompraCommandValidator : AbstractValidator<CrearOrdenCompraCommand>
{
    public CrearOrdenCompraCommandValidator()
    {
        RuleFor(x => x.ProveedorId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty().WithMessage("La orden de compra debe tener al menos un ítem.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductoId).NotEmpty();
            item.RuleFor(x => x.Cantidad).GreaterThan(0);
            item.RuleFor(x => x.PrecioUnitario).GreaterThanOrEqualTo(0);
        });
    }
}

public class CrearOrdenCompraCommandHandler : IRequestHandler<CrearOrdenCompraCommand, Guid>
{
    private readonly IOrdenCompraRepository _ordenCompraRepository;
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUserContext;

    public CrearOrdenCompraCommandHandler(
        IOrdenCompraRepository ordenCompraRepository,
        IProveedorRepository proveedorRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserContext currentUserContext)
    {
        _ordenCompraRepository = ordenCompraRepository;
        _proveedorRepository = proveedorRepository;
        _unitOfWork = unitOfWork;
        _currentUserContext = currentUserContext;
    }

    public async Task<Guid> Handle(CrearOrdenCompraCommand request, CancellationToken cancellationToken)
    {
        var proveedor = await _proveedorRepository.ObtenerPorIdAsync(request.ProveedorId);
        if (proveedor == null)
            throw new Exception("Proveedor no encontrado.");

        var usuarioId = _currentUserContext.UsuarioId ?? Guid.Empty;
        var totalMonto = request.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

        var ordenCompra = new OrdenCompra(
            Guid.NewGuid(),
            request.ProveedorId,
            DateTime.UtcNow,
            new Moneda(totalMonto),
            usuarioId,
            request.Nota);

        foreach (var itemDto in request.Items)
        {
            ordenCompra.AgregarItem(
                Guid.NewGuid(),
                itemDto.ProductoId,
                itemDto.Cantidad,
                new Moneda(itemDto.PrecioUnitario));
        }

        _ordenCompraRepository.Agregar(ordenCompra);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ordenCompra.Id;
    }
}
