using FluentValidation;

namespace GalponERP.Application.Inventario.Commands.RegistrarPagoCompra;

public class RegistrarPagoCompraCommandValidator : AbstractValidator<RegistrarPagoCompraCommand>
{
    public RegistrarPagoCompraCommandValidator()
    {
        RuleFor(x => x.CompraId).NotEmpty();
        RuleFor(x => x.Monto).GreaterThan(0);
        RuleFor(x => x.FechaPago).NotEmpty();
        RuleFor(x => x.MetodoPago).IsInEnum();
        RuleFor(x => x.UsuarioId).NotEmpty();
    }
}
