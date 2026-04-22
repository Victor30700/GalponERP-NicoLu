using FluentValidation;

namespace GalponERP.Application.Ventas.Commands.ActualizarVenta;

public class ActualizarVentaCommandValidator : AbstractValidator<ActualizarVentaCommand>
{
    public ActualizarVentaCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El ID es obligatorio.");
        
        RuleFor(x => x.CantidadPollos)
            .GreaterThan(0).WithMessage("La cantidad de pollos debe ser mayor a 0.");

        RuleFor(x => x.PesoTotalVendido)
            .GreaterThan(0).WithMessage("El peso total vendido debe ser mayor a 0.");

        RuleFor(x => x.PrecioPorKilo)
            .GreaterThan(0).WithMessage("El precio por kilo debe ser mayor a 0.");

        RuleFor(x => x.Version).NotEmpty().WithMessage("La versión de concurrencia es obligatoria.");
    }
}
