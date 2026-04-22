using FluentValidation;

namespace GalponERP.Application.Inventario.Commands.ActualizarMovimiento;

public class ActualizarMovimientoCommandValidator : AbstractValidator<ActualizarMovimientoCommand>
{
    public ActualizarMovimientoCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El ID es obligatorio.");
        RuleFor(x => x.ProductoId).NotEmpty().WithMessage("El producto es obligatorio.");
        RuleFor(x => x.Cantidad).NotEqual(0).WithMessage("La cantidad no puede ser cero.");
        RuleFor(x => x.Fecha).NotEmpty().WithMessage("La fecha es obligatoria.");
        RuleFor(x => x.UsuarioId).NotEmpty().WithMessage("El usuario es obligatorio.");
        RuleFor(x => x.Version).NotEmpty().WithMessage("La versión de concurrencia es obligatoria.");
    }
}
