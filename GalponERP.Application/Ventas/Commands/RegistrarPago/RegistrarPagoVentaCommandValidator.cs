using FluentValidation;

namespace GalponERP.Application.Ventas.Commands.RegistrarPago;

public class RegistrarPagoVentaCommandValidator : AbstractValidator<RegistrarPagoVentaCommand>
{
    public RegistrarPagoVentaCommandValidator()
    {
        RuleFor(x => x.VentaId).NotEmpty();
        RuleFor(x => x.Monto).GreaterThan(0).WithMessage("El monto del abono debe ser mayor a cero.");
        RuleFor(x => x.FechaPago).NotEmpty();
        RuleFor(x => x.MetodoPago).IsInEnum().WithMessage("Método de pago inválido.");
        RuleFor(x => x.UsuarioId).NotEmpty();
    }
}
