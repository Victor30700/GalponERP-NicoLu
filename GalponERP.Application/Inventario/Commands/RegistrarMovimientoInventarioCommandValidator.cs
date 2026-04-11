using FluentValidation;

namespace GalponERP.Application.Inventario.Commands.RegistrarMovimiento;

public class RegistrarMovimientoInventarioCommandValidator : AbstractValidator<RegistrarMovimientoInventarioCommand>
{
    public RegistrarMovimientoInventarioCommandValidator()
    {
        RuleFor(x => x.ProductoId)
            .NotEmpty().WithMessage("El ID del producto es obligatorio.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0).WithMessage("La cantidad del movimiento debe ser mayor a cero.");

        RuleFor(x => x.Fecha)
            .NotEmpty().WithMessage("La fecha del movimiento es obligatoria.")
            .LessThanOrEqualTo(x => DateTime.UtcNow.AddMinutes(5)).WithMessage("La fecha no puede ser futura.");
    }
}
