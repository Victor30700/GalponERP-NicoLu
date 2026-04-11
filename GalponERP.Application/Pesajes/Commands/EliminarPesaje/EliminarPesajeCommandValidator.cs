using FluentValidation;

namespace GalponERP.Application.Pesajes.Commands.EliminarPesaje;

public class EliminarPesajeCommandValidator : AbstractValidator<EliminarPesajeCommand>
{
    public EliminarPesajeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El ID del registro es obligatorio.");
    }
}
