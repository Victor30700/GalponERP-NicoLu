using FluentValidation;

namespace GalponERP.Application.Lotes.Commands.CrearLote;

public class CrearLoteCommandValidator : AbstractValidator<CrearLoteCommand>
{
    public CrearLoteCommandValidator()
    {
        RuleFor(x => x.GalponId)
            .NotEmpty().WithMessage("El ID del galpón es obligatorio.");

        RuleFor(x => x.FechaIngreso)
            .NotEmpty().WithMessage("La fecha de ingreso es obligatoria.")
            .LessThanOrEqualTo(x => DateTime.UtcNow.AddMinutes(5)).WithMessage("La fecha de ingreso no puede ser futura.");

        RuleFor(x => x.CantidadInicial)
            .GreaterThan(0).WithMessage("La cantidad inicial del lote debe ser mayor a cero.");

        RuleFor(x => x.CostoUnitarioPollito)
            .GreaterThanOrEqualTo(0).WithMessage("El costo unitario del pollito no puede ser negativo.");
    }
}
