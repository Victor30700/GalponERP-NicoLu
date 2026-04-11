using FluentValidation;

namespace GalponERP.Application.Lotes.Commands.ActualizarLote;

public class ActualizarLoteCommandValidator : AbstractValidator<ActualizarLoteCommand>
{
    public ActualizarLoteCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El ID del lote es obligatorio.");
        RuleFor(x => x.GalponId).NotEmpty().WithMessage("El ID del galpón es obligatorio.");
        RuleFor(x => x.FechaIngreso).NotEmpty().LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de ingreso no puede ser futura.");
        RuleFor(x => x.CantidadInicial).GreaterThan(0).WithMessage("La cantidad inicial debe ser mayor a cero.");
        RuleFor(x => x.CostoUnitarioPollito).GreaterThanOrEqualTo(0).WithMessage("El costo unitario debe ser mayor o igual a cero.");
    }
}
