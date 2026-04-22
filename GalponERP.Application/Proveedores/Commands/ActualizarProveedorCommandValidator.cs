using FluentValidation;

namespace GalponERP.Application.Proveedores.Commands.ActualizarProveedor;

public class ActualizarProveedorCommandValidator : AbstractValidator<ActualizarProveedorCommand>
{
    public ActualizarProveedorCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El ID es obligatorio.");
        
        RuleFor(x => x.RazonSocial)
            .NotEmpty().WithMessage("La razón social es obligatoria.")
            .MaximumLength(150).WithMessage("La razón social no puede exceder los 150 caracteres.");

        RuleFor(x => x.NitRuc)
            .NotEmpty().WithMessage("El NIT/RUC es obligatorio.")
            .MaximumLength(20).WithMessage("El NIT/RUC no puede exceder los 20 caracteres.");

        RuleFor(x => x.Version).NotEmpty().WithMessage("La versión de concurrencia es obligatoria.");
    }
}
