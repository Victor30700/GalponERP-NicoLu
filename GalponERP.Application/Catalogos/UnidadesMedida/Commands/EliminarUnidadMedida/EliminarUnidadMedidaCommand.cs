using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Catalogos.UnidadesMedida.Commands.EliminarUnidadMedida;

public record EliminarUnidadMedidaCommand(Guid Id) : IRequest, IAuditableCommand;

public class EliminarUnidadMedidaCommandHandler : IRequestHandler<EliminarUnidadMedidaCommand>
{
    private readonly IUnidadMedidaRepository _unidadMedidaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarUnidadMedidaCommandHandler(IUnidadMedidaRepository unidadMedidaRepository, IUnitOfWork unitOfWork)
    {
        _unidadMedidaRepository = unidadMedidaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EliminarUnidadMedidaCommand request, CancellationToken cancellationToken)
    {
        var unidad = await _unidadMedidaRepository.ObtenerPorIdAsync(request.Id);
        if (unidad == null)
            throw new Exception("Unidad de medida no encontrada");

        unidad.Eliminar();
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
