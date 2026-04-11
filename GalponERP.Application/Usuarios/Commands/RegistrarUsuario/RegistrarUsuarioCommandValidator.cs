using FluentValidation;
using GalponERP.Domain.Entities;

namespace GalponERP.Application.Usuarios.Commands.RegistrarUsuario;

public class RegistrarUsuarioCommandValidator : AbstractValidator<RegistrarUsuarioCommand>
{
    public RegistrarUsuarioCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Apellidos).MaximumLength(100);
        RuleFor(x => x.Direccion).MaximumLength(200);
        RuleFor(x => x.Profesion).MaximumLength(100);
        RuleFor(x => x.Rol).IsInEnum()
            .WithMessage("El rol no es un valor válido.");
        RuleFor(x => x.FechaNacimiento).LessThan(DateTime.Now).WithMessage("La fecha de nacimiento debe ser en el pasado.");
    }
}
