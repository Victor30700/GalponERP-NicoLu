using FluentValidation;

namespace GalponERP.Application.Ventas.Commands.RegistrarVentaParcial;

public class RegistrarVentaParcialValidator : AbstractValidator<RegistrarVentaParcialCommand>
{
    public RegistrarVentaParcialValidator()
    {
        RuleFor(x => x.LoteId)
            .NotEmpty().WithMessage("El ID del lote es obligatorio.");

        RuleFor(x => x.ClienteId)
            .NotEmpty().WithMessage("El ID del cliente es obligatorio.");

        RuleFor(x => x.Fecha)
            .NotEmpty().WithMessage("La fecha de venta es obligatoria.")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha no puede ser futura.");

        RuleFor(x => x.CantidadPollos)
            .GreaterThan(0).WithMessage("La cantidad de pollos debe ser mayor a cero.");

        RuleFor(x => x.PesoTotalVendido)
            .GreaterThan(0).WithMessage("El peso total vendido debe ser mayor a cero.");

        RuleFor(x => x.PrecioPorKilo)
            .GreaterThanOrEqualTo(0).WithMessage("El precio por kilo no puede ser negativo.");
    }
}
