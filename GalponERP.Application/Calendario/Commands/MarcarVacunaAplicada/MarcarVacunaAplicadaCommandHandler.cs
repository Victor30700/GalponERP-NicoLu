using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Calendario.Commands.MarcarVacunaAplicada;

public class MarcarVacunaAplicadaCommandHandler : IRequestHandler<MarcarVacunaAplicadaCommand>
{
    private readonly ICalendarioSanitarioRepository _calendarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarcarVacunaAplicadaCommandHandler(
        ICalendarioSanitarioRepository calendarioRepository,
        IUnitOfWork unitOfWork)
    {
        _calendarioRepository = calendarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(MarcarVacunaAplicadaCommand request, CancellationToken cancellationToken)
    {
        var actividad = await _calendarioRepository.ObtenerPorIdAsync(request.ActividadId);

        if (actividad == null)
        {
            throw new Exception("Actividad del calendario no encontrada.");
        }

        actividad.MarcarComoAplicado();

        _calendarioRepository.Actualizar(actividad);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
