using FluentValidation;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Calendario.Commands;

public record ReprogramarActividadCommand(
    Guid ActividadId,
    DateTime NuevaFecha,
    string Justificacion) : IRequest, IAuditableCommand;

public class ReprogramarActividadCommandValidator : AbstractValidator<ReprogramarActividadCommand>
{
    public ReprogramarActividadCommandValidator()
    {
        RuleFor(x => x.ActividadId).NotEmpty();
        RuleFor(x => x.NuevaFecha).NotEmpty();
        RuleFor(x => x.Justificacion).NotEmpty().MaximumLength(500);
    }
}

public class ReprogramarActividadCommandHandler : IRequestHandler<ReprogramarActividadCommand>
{
    private readonly ICalendarioSanitarioRepository _calendarioRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReprogramarActividadCommandHandler(
        ICalendarioSanitarioRepository calendarioRepository,
        ILoteRepository loteRepository,
        IUnitOfWork unitOfWork)
    {
        _calendarioRepository = calendarioRepository;
        _loteRepository = loteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReprogramarActividadCommand request, CancellationToken cancellationToken)
    {
        var actividad = await _calendarioRepository.ObtenerPorIdAsync(request.ActividadId);
        if (actividad == null)
            throw new Exception("Actividad no encontrada.");

        var lote = await _loteRepository.ObtenerPorIdAsync(actividad.LoteId);
        if (lote == null)
            throw new Exception("Lote no encontrado.");

        var nuevoDiaDeAplicacion = (int)(request.NuevaFecha.Date - lote.FechaIngreso.Date).TotalDays + 1;
        if (nuevoDiaDeAplicacion <= 0)
            throw new Exception("La nueva fecha no puede ser anterior a la fecha de ingreso del lote.");

        actividad.Reprogramar(nuevoDiaDeAplicacion, request.Justificacion);

        _calendarioRepository.Actualizar(actividad);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
