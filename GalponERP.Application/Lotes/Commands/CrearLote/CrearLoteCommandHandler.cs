using GalponERP.Application.Interfaces;
using GalponERP.Application.Exceptions;
using FluentValidation.Results;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Lotes.Commands.CrearLote;

public class CrearLoteCommandHandler : IRequestHandler<CrearLoteCommand, Guid>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IGalponRepository _galponRepository;
    private readonly ICalendarioSanitarioRepository _calendarioRepository;
    private readonly IPlantillaSanitariaRepository _plantillaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearLoteCommandHandler(
        ILoteRepository loteRepository,
        IGalponRepository galponRepository,
        ICalendarioSanitarioRepository calendarioRepository,
        IPlantillaSanitariaRepository plantillaRepository,
        IUnitOfWork unitOfWork)
    {
        _loteRepository = loteRepository;
        _galponRepository = galponRepository;
        _calendarioRepository = calendarioRepository;
        _plantillaRepository = plantillaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CrearLoteCommand request, CancellationToken cancellationToken)
    {
        // Validar que el galpón exista
        var galpon = await _galponRepository.ObtenerPorIdAsync(request.GalponId);
        if (galpon == null)
        {
            throw new ValidationException(new List<ValidationFailure> 
            { 
                new ValidationFailure("GalponId", $"El galpón con ID {request.GalponId} no existe.") 
            });
        }

        // Asegurar que la fecha sea UTC para PostgreSQL
        var fechaUtc = request.FechaIngreso.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(request.FechaIngreso, DateTimeKind.Utc) 
            : request.FechaIngreso.ToUniversalTime();

        var loteId = Guid.NewGuid();
        var lote = new Lote(
            loteId,
            request.Nombre,
            request.GalponId,
            fechaUtc,
            request.CantidadInicial,
            new Moneda(request.CostoUnitarioPollito)
        );

        _loteRepository.Agregar(lote);

        // Generar calendario
        if (request.PlantillaSanitariaId.HasValue)
        {
            var plantilla = await _plantillaRepository.ObtenerPorIdAsync(request.PlantillaSanitariaId.Value);
            if (plantilla != null)
            {
                foreach (var actividad in plantilla.Actividades)
                {
                    var item = new CalendarioSanitario(
                        id: Guid.NewGuid(),
                        loteId: loteId,
                        diaDeAplicacion: actividad.DiaDeAplicacion,
                        descripcionTratamiento: actividad.Descripcion,
                        tipo: actividad.TipoActividad,
                        productoIdRecomendado: actividad.ProductoIdRecomendado,
                        cantidadRecomendada: actividad.CantidadRecomendada
                    );
                    _calendarioRepository.Agregar(item);
                }
            }
        }
        else
        {
            // Calendario base si no hay plantilla (Mantener retrocompatibilidad o fallback)
            var vacunas = new List<CalendarioSanitario>
            {
                new(id: Guid.NewGuid(), loteId: loteId, diaDeAplicacion: 7, descripcionTratamiento: "Vacuna Newcastle (Cepa LaSota)", tipo: TipoActividad.Vacuna),
                new(id: Guid.NewGuid(), loteId: loteId, diaDeAplicacion: 14, descripcionTratamiento: "Vacuna Gumboro", tipo: TipoActividad.Vacuna)
            };

            foreach (var vacuna in vacunas)
            {
                _calendarioRepository.Agregar(vacuna);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return lote.Id;
    }
}
