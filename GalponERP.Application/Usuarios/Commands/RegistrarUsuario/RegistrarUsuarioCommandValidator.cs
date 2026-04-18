using FluentValidation;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;

namespace GalponERP.Application.Usuarios.Commands.RegistrarUsuario;

public class RegistrarUsuarioCommandValidator : AbstractValidator<RegistrarUsuarioCommand>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public RegistrarUsuarioCommandValidator(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;

        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Apellidos).MaximumLength(100);
        RuleFor(x => x.Direccion).MaximumLength(200);
        RuleFor(x => x.Profesion).MaximumLength(100);
        RuleFor(x => x.Rol).IsInEnum()
            .WithMessage("El rol no es un valor válido.");
        RuleFor(x => x.FechaNacimiento)
            .LessThanOrEqualTo(DateTime.Now.AddDays(1))
            .WithMessage("La fecha de nacimiento no puede ser una fecha futura.");

        RuleFor(x => x.Telefono)
            .MustAsync(BeUniquePhone)
            .WithMessage(x => $"El número de teléfono {x.Telefono} ya está registrado con otro usuario.")
            .When(x => !string.IsNullOrWhiteSpace(x.Telefono));
    }

    private async Task<bool> BeUniquePhone(string? telefono, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(telefono)) return true;
        
        string telefonoNormalizado = telefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        var user = await _usuarioRepository.ObtenerPorTelefonoAsync(telefonoNormalizado);
        return user == null;
    }
}
