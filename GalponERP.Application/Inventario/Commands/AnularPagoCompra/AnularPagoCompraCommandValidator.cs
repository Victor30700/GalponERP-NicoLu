using FluentValidation;

namespace GalponERP.Application.Inventario.Commands.AnularPagoCompra;

public class AnularPagoCompraCommandValidator : AbstractValidator<AnularPagoCompraCommand>
{
    public AnularPagoCompraCommandValidator()
    {
        RuleFor(v => v.CompraId).NotEmpty();
        RuleFor(v => v.PagoId).NotEmpty();
        RuleFor(v => v.UsuarioId).NotEmpty();
    }
}
