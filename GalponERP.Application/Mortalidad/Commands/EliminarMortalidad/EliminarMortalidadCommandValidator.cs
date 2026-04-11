using FluentValidation;

namespace GalponERP.Application.Mortalidad.Commands.EliminarMortalidad;

public class EliminarMortalidadCommandValidator : AbstractValidator<EliminarMortalidadCommand>
{
    public EliminarMortalidadCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del registro es obligatorio.");
    }
}
