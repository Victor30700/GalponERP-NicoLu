using FluentValidation;

namespace GalponERP.Application.Lotes.Commands.EliminarLote;

public class EliminarLoteCommandValidator : AbstractValidator<EliminarLoteCommand>
{
    public EliminarLoteCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El ID del lote es obligatorio.");
    }
}
