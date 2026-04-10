using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Lotes.Commands.CrearLote;

public class CrearLoteCommandHandler : IRequestHandler<CrearLoteCommand, Guid>
{
    private readonly ILoteRepository _loteRepository;
    private readonly ICalendarioSanitarioRepository _calendarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearLoteCommandHandler(
        ILoteRepository loteRepository,
        ICalendarioSanitarioRepository calendarioRepository,
        IUnitOfWork unitOfWork)
    {
        _loteRepository = loteRepository;
        _calendarioRepository = calendarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CrearLoteCommand request, CancellationToken cancellationToken)
    {
        var loteId = Guid.NewGuid();
        var lote = new Lote(
            loteId,
            request.FechaIngreso,
            request.CantidadInicial,
            new Moneda(request.CostoUnitarioPollito)
        );

        _loteRepository.Agregar(lote);

        // Tarea 8.4: Generar calendario base
        var vacunas = new List<CalendarioSanitario>
        {
            new(Guid.NewGuid(), loteId, 7, "Vacuna Newcastle (Cepa LaSota)"),
            new(Guid.NewGuid(), loteId, 14, "Vacuna Gumboro")
        };

        foreach (var vacuna in vacunas)
        {
            _calendarioRepository.Agregar(vacuna);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return lote.Id;
    }
}
