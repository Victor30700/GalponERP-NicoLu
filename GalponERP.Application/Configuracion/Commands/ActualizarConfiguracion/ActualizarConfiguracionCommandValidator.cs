using FluentValidation;

namespace GalponERP.Application.Configuracion.Commands.ActualizarConfiguracion;

public class ActualizarConfiguracionCommandValidator : AbstractValidator<ActualizarConfiguracionCommand>
{
    public ActualizarConfiguracionCommandValidator()
    {
        RuleFor(x => x.NombreEmpresa).NotEmpty().MaximumLength(200).WithMessage("El nombre de la empresa es obligatorio.");
        RuleFor(x => x.Nit).NotEmpty().MaximumLength(50).WithMessage("El NIT/RUC es obligatorio.");
        RuleFor(x => x.MonedaPorDefecto).NotEmpty().MaximumLength(10).WithMessage("La moneda es obligatoria.");
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("El email no es válido.");
        RuleFor(x => x.Version).NotEmpty().WithMessage("La versión de concurrencia es obligatoria.");
    }
}
