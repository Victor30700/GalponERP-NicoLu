using FluentValidation;

namespace GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula;

public class ActualizarFormulaCommandValidator : AbstractValidator<ActualizarFormulaCommand>
{
    public ActualizarFormulaCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El ID es obligatorio.");
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100).WithMessage("El nombre es obligatorio.");
        RuleFor(x => x.Etapa).NotEmpty().MaximumLength(50).WithMessage("La etapa es obligatoria.");
        RuleFor(x => x.CantidadBase).GreaterThan(0).WithMessage("La cantidad base debe ser mayor a cero.");
        RuleFor(x => x.Detalles).NotEmpty().WithMessage("La fórmula debe tener al menos un ingrediente.");
        RuleFor(x => x.Version).NotEmpty().WithMessage("La versión de concurrencia es obligatoria.");
    }
}
