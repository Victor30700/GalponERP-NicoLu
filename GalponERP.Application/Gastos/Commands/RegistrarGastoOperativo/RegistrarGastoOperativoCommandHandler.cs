using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Gastos.Commands.RegistrarGastoOperativo;

public class RegistrarGastoOperativoCommandHandler : IRequestHandler<RegistrarGastoOperativoCommand, Guid>
{
    private readonly IGastoOperativoRepository _gastoOperativoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarGastoOperativoCommandHandler(
        IGastoOperativoRepository gastoOperativoRepository,
        IUnitOfWork unitOfWork)
    {
        _gastoOperativoRepository = gastoOperativoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegistrarGastoOperativoCommand request, CancellationToken cancellationToken)
    {
        var monto = new Moneda(request.Monto);
        
        // Asegurar que la fecha sea UTC para PostgreSQL
        var fechaUtc = request.Fecha.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(request.Fecha, DateTimeKind.Utc) 
            : request.Fecha.ToUniversalTime();

        var gasto = new GastoOperativo(
            Guid.NewGuid(),
            request.GalponId,
            request.LoteId,
            request.Descripcion,
            monto,
            fechaUtc,
            request.TipoGasto,
            request.UsuarioId
        );

        _gastoOperativoRepository.Agregar(gasto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return gasto.Id;
    }
}
