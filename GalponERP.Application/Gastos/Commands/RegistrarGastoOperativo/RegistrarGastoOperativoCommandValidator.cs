using FluentValidation;

namespace GalponERP.Application.Gastos.Commands.RegistrarGastoOperativo;

public class RegistrarGastoOperativoCommandValidator : AbstractValidator<RegistrarGastoOperativoCommand>
{
    public RegistrarGastoOperativoCommandValidator()
    {
        RuleFor(x => x.GalponId)
            .NotEmpty().WithMessage("El ID del galpón es obligatorio.");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción del gasto es obligatoria.")
            .MaximumLength(500).WithMessage("La descripción no puede exceder los 500 caracteres.");

        RuleFor(x => x.Monto)
            .GreaterThan(0).WithMessage("El monto del gasto debe ser mayor a cero.");

        RuleFor(x => x.Fecha)
            .NotEmpty().WithMessage("La fecha es obligatoria.")
            .LessThanOrEqualTo(x => DateTime.UtcNow.AddMinutes(5)).WithMessage("La fecha no puede ser futura.");

        RuleFor(x => x.TipoGasto)
            .NotEmpty().WithMessage("El tipo de gasto es obligatorio.");
    }
}
