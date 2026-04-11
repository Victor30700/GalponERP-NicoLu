using FluentValidation;

namespace GalponERP.Application.Gastos.Commands.ActualizarGastoOperativo;

public class ActualizarGastoOperativoCommandValidator : AbstractValidator<ActualizarGastoOperativoCommand>
{
    public ActualizarGastoOperativoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del gasto es requerido.");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción es requerida.")
            .MaximumLength(200).WithMessage("La descripción no puede exceder los 200 caracteres.");

        RuleFor(x => x.Monto)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a cero.");

        RuleFor(x => x.Fecha)
            .NotEmpty().WithMessage("La fecha es requerida.");

        RuleFor(x => x.TipoGasto)
            .NotEmpty().WithMessage("El tipo de gasto es requerido.");
    }
}
