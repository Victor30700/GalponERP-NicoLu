using FluentValidation;

namespace GalponERP.Application.Pesajes.Commands.ActualizarPesaje;

public class ActualizarPesajeCommandValidator : AbstractValidator<ActualizarPesajeCommand>
{
    public ActualizarPesajeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El ID del registro es obligatorio.");
        RuleFor(x => x.Fecha).NotEmpty()
            .LessThanOrEqualTo(x => DateTime.UtcNow.AddMinutes(5)).WithMessage("La fecha no puede ser futura.");
        RuleFor(x => x.PesoPromedioGramos).GreaterThan(0).WithMessage("El peso promedio debe ser mayor a cero.");
        RuleFor(x => x.CantidadMuestreada).GreaterThan(0).WithMessage("La cantidad muestreada debe ser mayor a cero.");
    }
}
