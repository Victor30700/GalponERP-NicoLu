using FluentValidation;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;

namespace GalponERP.Application.Usuarios.Commands.ActualizarUsuario;

public class ActualizarUsuarioCommandValidator : AbstractValidator<ActualizarUsuarioCommand>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public ActualizarUsuarioCommandValidator(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;

        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Apellidos).MaximumLength(100);
        RuleFor(x => x.Direccion).MaximumLength(200);
        RuleFor(x => x.Profesion).MaximumLength(100);
        RuleFor(x => x.Rol).IsInEnum()
            .WithMessage("El rol no es un valor válido.");
        RuleFor(x => x.FechaNacimiento).LessThan(DateTime.Now).WithMessage("La fecha de nacimiento debe ser en el pasado.");

        RuleFor(x => x.Telefono)
            .MustAsync(BeUniquePhone)
            .WithMessage(x => $"El número de teléfono {x.Telefono} ya está registrado con otro usuario.")
            .When(x => !string.IsNullOrWhiteSpace(x.Telefono));
    }

    private async Task<bool> BeUniquePhone(ActualizarUsuarioCommand command, string? telefono, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(telefono)) return true;
        
        string telefonoNormalizado = telefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        var user = await _usuarioRepository.ObtenerPorTelefonoAsync(telefonoNormalizado);
        
        // Es válido si no existe, o si el que existe es el mismo usuario que estamos editando
        return user == null || user.Id == command.Id;
    }
}
