using FluentValidation;

namespace GalponERP.Application.Pesajes.Commands.RegistrarPesaje;

public class RegistrarPesajeCommandValidator : AbstractValidator<RegistrarPesajeCommand>
{
    public RegistrarPesajeCommandValidator()
    {
        RuleFor(x => x.LoteId).NotEmpty();
        RuleFor(x => x.Fecha).NotEmpty().LessThanOrEqualTo(DateTime.Now);
        RuleFor(x => x.PesoPromedioGramos).GreaterThan(0);
        RuleFor(x => x.CantidadMuestreada).GreaterThan(0);
    }
}
