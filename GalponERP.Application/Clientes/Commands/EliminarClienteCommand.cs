using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Clientes.Commands.EliminarCliente;

public record EliminarClienteCommand(Guid Id) : IRequest;

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
        var cliente = await _clienteRepository.ObtenerPorIdAsync(request.Id);

        if (cliente == null)
        {
            throw new Exception("Cliente no encontrado.");
        }

        cliente.Desactivar();

        _clienteRepository.Actualizar(cliente);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
