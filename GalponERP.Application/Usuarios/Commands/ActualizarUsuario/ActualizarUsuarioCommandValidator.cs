using FluentValidation;
using GalponERP.Domain.Entities;

namespace GalponERP.Application.Usuarios.Commands.ActualizarUsuario;

public class ActualizarUsuarioCommandValidator : AbstractValidator<ActualizarUsuarioCommand>
{
    public ActualizarUsuarioCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Apellidos).MaximumLength(100);
        RuleFor(x => x.Direccion).MaximumLength(200);
        RuleFor(x => x.Profesion).MaximumLength(100);
        RuleFor(x => x.Rol).NotEmpty().Must(rol => RolesGalpon.All.Contains(rol))
            .WithMessage($"El rol debe ser uno de los siguientes: {string.Join(", ", RolesGalpon.All)}");
        RuleFor(x => x.FechaNacimiento).LessThan(DateTime.Now).WithMessage("La fecha de nacimiento debe ser en el pasado.");
    }
}
