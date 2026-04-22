using FluentValidation;
using GalponERP.Domain.Interfaces.Repositories;

namespace GalponERP.Application.Clientes.Commands.CrearCliente;

public class CrearClienteCommandValidator : AbstractValidator<CrearClienteCommand>
{
    private readonly IClienteRepository _clienteRepository;

    public CrearClienteCommandValidator(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

        RuleFor(x => x.Ruc)
            .NotEmpty().WithMessage("El RUC es obligatorio.")
            .MaximumLength(20).WithMessage("El RUC no puede exceder los 20 caracteres.")
            .MustAsync(BeUniqueRuc).WithMessage("El RUC ya está registrado en el sistema (incluso si fue eliminado).");
    }

    private async Task<bool> BeUniqueRuc(string ruc, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(ruc)) return true;
        
        var existingCliente = await _clienteRepository.ObtenerPorRucAsync(ruc);
        return existingCliente == null;
    }
}
