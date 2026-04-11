using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Gastos.Commands.EliminarGastoOperativo;

public class EliminarGastoOperativoCommandHandler : IRequestHandler<EliminarGastoOperativoCommand, Unit>
{
    private readonly IGastoOperativoRepository _gastoOperativoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarGastoOperativoCommandHandler(
        IGastoOperativoRepository gastoOperativoRepository,
        IUnitOfWork unitOfWork)
    {
        _gastoOperativoRepository = gastoOperativoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(EliminarGastoOperativoCommand request, CancellationToken cancellationToken)
    {
        var gasto = await _gastoOperativoRepository.ObtenerPorIdAsync(request.Id);

        if (gasto == null || !gasto.IsActive)
        {
            throw new ArgumentException("El gasto operativo no existe o ya ha sido eliminado.", nameof(request.Id));
        }

        gasto.Eliminar();
        gasto.SetAuditoriaModificacion(DateTime.UtcNow, request.UsuarioId);

        _gastoOperativoRepository.Actualizar(gasto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
