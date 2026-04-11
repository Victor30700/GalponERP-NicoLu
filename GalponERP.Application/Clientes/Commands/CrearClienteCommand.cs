using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Clientes.Commands.CrearCliente;

public record CrearClienteCommand(
    string Nombre,
    string Ruc,
    string? Direccion,
    string? Telefono) : IRequest<Guid>;

public class CrearClienteCommandHandler : IRequestHandler<CrearClienteCommand, Guid>
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearClienteCommandHandler(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
    {
        _clienteRepository = clienteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CrearClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = new Cliente(
            Guid.NewGuid(),
            request.Nombre,
            request.Ruc,
            request.Direccion,
            request.Telefono);

        _clienteRepository.Agregar(cliente);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return cliente.Id;
    }
}
