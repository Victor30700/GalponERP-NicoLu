using GalponERP.Application.Interfaces;
using GalponERP.Application.Exceptions;
using FluentValidation.Results;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Lotes.Commands.ActualizarLote;

public class ActualizarLoteCommandHandler : IRequestHandler<ActualizarLoteCommand, Unit>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IGalponRepository _galponRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarLoteCommandHandler(ILoteRepository loteRepository, IGalponRepository galponRepository, IUnitOfWork unitOfWork)
    {
        _loteRepository = loteRepository;
        _galponRepository = galponRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ActualizarLoteCommand request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.Id);

        if (lote == null)
        {
            throw new KeyNotFoundException("Lote no encontrado.");
        }

        // Chequeo de concurrencia optimista
        if (!string.IsNullOrEmpty(request.Version) && lote.Version.ToString() != request.Version)
        {
            throw new ConcurrencyException();
        }

        // Validar que el galpón exista
        var galpon = await _galponRepository.ObtenerPorIdAsync(request.GalponId);
        if (galpon == null)
        {
            throw new ValidationException(new List<ValidationFailure> 
            { 
                new ValidationFailure("GalponId", $"El galpón con ID {request.GalponId} no existe.") 
            });
        }

        // Validación preventiva antes de llamar al dominio para asegurar 422 en lugar de 400 genérico
        int binnedActual = request.CantidadInicial - lote.MortalidadAcumulada - lote.PollosVendidos;
        if (binnedActual < 0)
        {
            throw new ValidationException(new List<ValidationFailure> 
            { 
                new ValidationFailure("CantidadInicial", "La nueva cantidad inicial es insuficiente para cubrir la mortalidad y ventas ya registradas.") 
            });
        }

        // 1. Actualizar datos iniciales (el método en la entidad valida el estado Activo)
        lote.ActualizarDatosIniciales(
            request.Nombre,
            request.GalponId,
            request.FechaIngreso,
            request.CantidadInicial,
            new Moneda(request.CostoUnitarioPollito)
        );

        lote.SetAuditoriaModificacion(DateTime.UtcNow, request.UsuarioId);

        // 2. Persistir
        _loteRepository.Actualizar(lote);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
