using FluentValidation;

namespace GalponERP.Application.Sanidad.Commands.ActualizarBienestar;

public class ActualizarBienestarCommandValidator : AbstractValidator<ActualizarBienestarCommand>
{
    public ActualizarBienestarCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El ID es obligatorio.");
        RuleFor(x => x.Fecha).NotEmpty().WithMessage("La fecha es obligatoria.");
        RuleFor(x => x.Temperatura).GreaterThanOrEqualTo(-50).LessThanOrEqualTo(60).WithMessage("La temperatura debe ser un valor válido.");
        RuleFor(x => x.Humedad).InclusiveBetween(0, 100).WithMessage("La humedad debe estar entre 0 y 100.");
        RuleFor(x => x.ConsumoAgua).GreaterThanOrEqualTo(0).WithMessage("El consumo de agua no puede ser negativo.");
        RuleFor(x => x.UsuarioId).NotEmpty().WithMessage("El usuario es obligatorio.");
        RuleFor(x => x.Version).NotEmpty().WithMessage("La versión de concurrencia es obligatoria.");
    }
}
