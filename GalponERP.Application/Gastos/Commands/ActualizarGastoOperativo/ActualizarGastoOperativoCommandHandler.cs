using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using GalponERP.Application.Exceptions;
using MediatR;

namespace GalponERP.Application.Gastos.Commands.ActualizarGastoOperativo;

public class ActualizarGastoOperativoCommandHandler : IRequestHandler<ActualizarGastoOperativoCommand, Unit>
{
    private readonly IGastoOperativoRepository _gastoOperativoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarGastoOperativoCommandHandler(
        IGastoOperativoRepository gastoOperativoRepository,
        IUnitOfWork unitOfWork)
    {
        _gastoOperativoRepository = gastoOperativoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ActualizarGastoOperativoCommand request, CancellationToken cancellationToken)
    {
        var gasto = await _gastoOperativoRepository.ObtenerPorIdAsync(request.Id);

        if (gasto == null || !gasto.IsActive)
        {
            throw new ArgumentException("El gasto operativo no existe o ya ha sido eliminado.", nameof(request.Id));
        }

        if (gasto.Version.ToString() != request.Version)
        {
            throw new ConcurrencyException();
        }

        var monto = new Moneda(request.Monto);

        gasto.Actualizar(
            request.Descripcion,
            monto,
            request.Fecha,
            request.TipoGasto,
            request.LoteId
        );

        gasto.SetAuditoriaModificacion(DateTime.UtcNow, request.UsuarioId);

        _gastoOperativoRepository.Actualizar(gasto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
