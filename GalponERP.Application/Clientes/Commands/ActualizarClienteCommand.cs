using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Clientes.Commands.ActualizarCliente;

public record ActualizarClienteCommand(
    Guid Id,
    string Nombre,
    string Ruc,
    string? Direccion,
    string? Telefono) : IRequest;

public class ActualizarClienteCommandHandler : IRequestHandler<ActualizarClienteCommand>
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarClienteCommandHandler(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
    {
        _clienteRepository = clienteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ActualizarClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObtenerPorIdIncluyendoInactivosAsync(request.Id);

        if (cliente == null)
        {
            throw new KeyNotFoundException($"Cliente con ID {request.Id} no encontrado.");
        }

        cliente.Actualizar(request.Nombre, request.Ruc, request.Direccion, request.Telefono);

        _clienteRepository.Actualizar(cliente);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
