using FluentValidation;

namespace GalponERP.Application.Lotes.Commands.CerrarLote;

public class CerrarLoteCommandValidator : AbstractValidator<CerrarLoteCommand>
{
    public CerrarLoteCommandValidator()
    {
        RuleFor(x => x.LoteId)
            .NotEmpty().WithMessage("El ID del lote es obligatorio para el cierre.");
    }
}
