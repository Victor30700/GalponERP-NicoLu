using FluentValidation;

namespace GalponERP.Application.Mortalidad.Commands.ActualizarMortalidad;

public class ActualizarMortalidadCommandValidator : AbstractValidator<ActualizarMortalidadCommand>
{
    public ActualizarMortalidadCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del registro es obligatorio.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0).WithMessage("La cantidad de bajas debe ser mayor a cero.");

        RuleFor(x => x.Causa)
            .NotEmpty().WithMessage("La causa de la baja es obligatoria.")
            .MaximumLength(200).WithMessage("La causa no puede exceder los 200 caracteres.");

        RuleFor(x => x.Fecha)
            .NotEmpty().WithMessage("La fecha es obligatoria.")
            .LessThanOrEqualTo(x => DateTime.UtcNow.AddMinutes(5)).WithMessage("La fecha no puede ser futura.");
    }
}
