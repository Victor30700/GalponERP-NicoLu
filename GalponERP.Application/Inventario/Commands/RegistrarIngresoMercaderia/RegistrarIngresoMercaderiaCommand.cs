using FluentValidation;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using MediatR;
using System.Text.Json.Serialization;

namespace GalponERP.Application.Inventario.Commands.RegistrarIngresoMercaderia;

public record RegistrarIngresoMercaderiaCommand(
    Guid ProductoId,
    decimal Cantidad,
    decimal CostoTotalCompra,
    string? Proveedor = null,
    string? Nota = null) : IRequest<Guid>
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}

public class RegistrarIngresoMercaderiaCommandValidator : AbstractValidator<RegistrarIngresoMercaderiaCommand>
{
    public RegistrarIngresoMercaderiaCommandValidator()
    {
        RuleFor(x => x.ProductoId).NotEmpty();
        RuleFor(x => x.Cantidad).GreaterThan(0);
        RuleFor(x => x.CostoTotalCompra).GreaterThanOrEqualTo(0);
    }
}

public class RegistrarIngresoMercaderiaCommandHandler : IRequestHandler<RegistrarIngresoMercaderiaCommand, Guid>
{
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarIngresoMercaderiaCommandHandler(IInventarioRepository inventarioRepository, IUnitOfWork unitOfWork)
    {
        _inventarioRepository = inventarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegistrarIngresoMercaderiaCommand request, CancellationToken cancellationToken)
    {
        var movimiento = new MovimientoInventario(
            Guid.NewGuid(),
            request.ProductoId,
            null,
            request.Cantidad,
            TipoMovimiento.Compra,
            DateTime.UtcNow,
            request.UsuarioId,
            request.Nota,
            new Moneda(request.CostoTotalCompra),
            request.Proveedor);

        _inventarioRepository.RegistrarMovimiento(movimiento);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return movimiento.Id;
    }
}
