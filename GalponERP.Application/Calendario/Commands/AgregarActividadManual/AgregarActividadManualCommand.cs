using FluentValidation;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Calendario.Commands;

public record AgregarActividadManualCommand(
    Guid LoteId,
    TipoActividad Tipo,
    DateTime FechaProgramada,
    string Descripcion,
    Guid? ProductoId = null) : IRequest<Guid>;

public class AgregarActividadManualCommandValidator : AbstractValidator<AgregarActividadManualCommand>
{
    public AgregarActividadManualCommandValidator()
    {
        RuleFor(x => x.LoteId).NotEmpty();
        RuleFor(x => x.Tipo).IsInEnum();
        RuleFor(x => x.FechaProgramada).NotEmpty();
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(250);
    }
}

public class AgregarActividadManualCommandHandler : IRequestHandler<AgregarActividadManualCommand, Guid>
{
    private readonly ICalendarioSanitarioRepository _calendarioRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AgregarActividadManualCommandHandler(
        ICalendarioSanitarioRepository calendarioRepository,
        ILoteRepository loteRepository,
        IUnitOfWork unitOfWork)
    {
        _calendarioRepository = calendarioRepository;
        _loteRepository = loteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AgregarActividadManualCommand request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null)
            throw new Exception("Lote no encontrado.");

        if (lote.Estado != EstadoLote.Activo)
            throw new Exception("Solo se pueden agregar actividades a lotes activos.");

        var diaDeAplicacion = (int)(request.FechaProgramada.Date - lote.FechaIngreso.Date).TotalDays + 1;
        if (diaDeAplicacion <= 0)
            throw new Exception("La fecha programada no puede ser anterior a la fecha de ingreso del lote.");

        var actividad = new CalendarioSanitario(
            Guid.NewGuid(),
            request.LoteId,
            diaDeAplicacion,
            request.Descripcion,
            request.Tipo,
            request.ProductoId,
            esManual: true,
            justificacion: "Actividad manual agregada por el usuario.");

        _calendarioRepository.Agregar(actividad);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return actividad.Id;
    }
}
