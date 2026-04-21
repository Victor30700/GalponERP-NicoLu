using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Clientes.Commands.EliminarCliente;

public record EliminarClienteCommand(Guid Id) : IRequest, IAuditableCommand;

public class EliminarClienteCommandHandler : IRequestHandler<EliminarClienteCommand>
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarClienteCommandHandler(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
    {
        _clienteRepository = clienteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EliminarClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObtenerPorIdIncluyendoInactivosAsync(request.Id);

        if (cliente == null)
        {
            throw new KeyNotFoundException($"Cliente con ID {request.Id} no encontrado.");
        }

        if (!cliente.IsActive)
        {
            return; // Ya está eliminado, no hacemos nada o lanzamos algo específico.
        }

        cliente.Desactivar();

        _clienteRepository.Actualizar(cliente);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
