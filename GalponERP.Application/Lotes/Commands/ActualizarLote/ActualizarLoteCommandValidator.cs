using FluentValidation;

namespace GalponERP.Application.Lotes.Commands.ActualizarLote;

public class ActualizarLoteCommandValidator : AbstractValidator<ActualizarLoteCommand>
{
    public ActualizarLoteCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El ID del lote es obligatorio.");
        
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del lote es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

        RuleFor(x => x.GalponId).NotEmpty().WithMessage("El ID del galpón es obligatorio.");
        
        RuleFor(x => x.FechaIngreso)
            .NotEmpty().WithMessage("La fecha de ingreso es obligatoria.")
            .LessThanOrEqualTo(x => DateTime.UtcNow.AddDays(1)).WithMessage("La fecha de ingreso no puede ser una fecha futura lejana.");

        RuleFor(x => x.CantidadInicial).GreaterThan(0).WithMessage("La cantidad inicial debe ser mayor a cero.");
        RuleFor(x => x.CostoUnitarioPollito).GreaterThanOrEqualTo(0).WithMessage("El costo unitario debe ser mayor o igual a cero.");
        
        RuleFor(x => x.Version)
            .NotEmpty().WithMessage("La versión de concurrencia es obligatoria.");
    }
}
