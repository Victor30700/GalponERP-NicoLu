using FluentValidation;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

using GalponERP.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace GalponERP.Application.Sanidad.Commands.RegistrarBienestar;

public record RegistrarBienestarCommand(
    Guid LoteId,
    DateTime Fecha,
    decimal? Temperatura = null,
    decimal? Humedad = null,
    decimal? ConsumoAgua = null,
    decimal? LecturaMedidor = null,
    string? Observaciones = null) : IRequest<Guid>;

public class RegistrarBienestarCommandValidator : AbstractValidator<RegistrarBienestarCommand>
{
    public RegistrarBienestarCommandValidator()
    {
        RuleFor(x => x.LoteId).NotEmpty();
        RuleFor(x => x.Fecha).NotEmpty();
    }
}

public class RegistrarBienestarCommandHandler : IRequestHandler<RegistrarBienestarCommand, Guid>
{
    private readonly IRegistroBienestarRepository _bienestarRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly ISanidadService _sanidadService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ILogger<RegistrarBienestarCommandHandler> _logger;

    public RegistrarBienestarCommandHandler(
        IRegistroBienestarRepository bienestarRepository,
        ILoteRepository loteRepository,
        ISanidadService sanidadService,
        IUnitOfWork unitOfWork,
        ICurrentUserContext currentUserContext,
        ILogger<RegistrarBienestarCommandHandler> logger)
    {
        _bienestarRepository = bienestarRepository;
        _loteRepository = loteRepository;
        _sanidadService = sanidadService;
        _unitOfWork = unitOfWork;
        _currentUserContext = currentUserContext;
        _logger = logger;
    }

    public async Task<Guid> Handle(RegistrarBienestarCommand request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null)
            throw new Exception("Lote no encontrado.");

        var usuarioId = _currentUserContext.UsuarioId ?? Guid.Empty;

        var registroExistente = await _bienestarRepository.ObtenerPorLoteYFechaAsync(request.LoteId, request.Fecha);
        RegistroBienestar registroAAnalizar;

        if (registroExistente != null)
        {
            registroExistente.Actualizar(
                request.Temperatura ?? registroExistente.Temperatura,
                request.Humedad ?? registroExistente.Humedad,
                request.ConsumoAgua ?? registroExistente.ConsumoAgua,
                request.Observaciones ?? registroExistente.Observaciones,
                lecturaMedidor: request.LecturaMedidor ?? registroExistente.LecturaMedidor);
            
            _bienestarRepository.Actualizar(registroExistente);
            registroAAnalizar = registroExistente;
        }
        else
        {
            var nuevoRegistro = new RegistroBienestar(
                Guid.NewGuid(),
                request.LoteId,
                request.Fecha,
                request.Temperatura,
                request.Humedad,
                request.ConsumoAgua,
                request.Observaciones,
                usuarioId,
                lecturaMedidor: request.LecturaMedidor);

            // Si se proporciona lectura del medidor pero no el consumo, intentar calcularlo
            if (request.LecturaMedidor.HasValue && !request.ConsumoAgua.HasValue)
            {
                var historialLectura = await _bienestarRepository.ObtenerHistorialPorLoteAsync(request.LoteId);
                var ultimoRegistro = historialLectura.OrderByDescending(h => h.Fecha).FirstOrDefault(h => h.Fecha < request.Fecha.Date);
                
                if (ultimoRegistro != null && ultimoRegistro.LecturaMedidor.HasValue)
                {
                    nuevoRegistro.CalcularConsumo(ultimoRegistro.LecturaMedidor.Value);
                }
            }

            _bienestarRepository.Agregar(nuevoRegistro);
            registroAAnalizar = nuevoRegistro;
        }

        // Blindaje Fase 2: Análisis de Alerta Sanitaria Proactiva
        if (registroAAnalizar.ConsumoAgua.HasValue && registroAAnalizar.ConsumoAgua > 0)
        {
            var historialParaAnalisis = await _bienestarRepository.ObtenerHistorialPorLoteAsync(request.LoteId);
            var (esAlerta, mensaje) = _sanidadService.AnalizarDesviacionConsumoAgua(
                request.LoteId, 
                registroAAnalizar.ConsumoAgua.Value, 
                historialParaAnalisis.Where(h => h.Id != registroAAnalizar.Id));

            if (esAlerta)
            {
                _logger.LogWarning(mensaje);
                // Aquí se podría integrar un servicio de notificaciones (WhatsApp, Email)
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return registroAAnalizar.Id;
    }
}
