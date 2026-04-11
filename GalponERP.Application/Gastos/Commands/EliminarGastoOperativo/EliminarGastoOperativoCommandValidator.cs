using FluentValidation;

namespace GalponERP.Application.Gastos.Commands.EliminarGastoOperativo;

public class EliminarGastoOperativoCommandValidator : AbstractValidator<EliminarGastoOperativoCommand>
{
    public EliminarGastoOperativoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del gasto es requerido.");
    }
}
