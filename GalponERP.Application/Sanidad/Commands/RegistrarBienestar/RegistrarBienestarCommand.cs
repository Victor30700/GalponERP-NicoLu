using FluentValidation;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Sanidad.Commands.RegistrarBienestar;

public record RegistrarBienestarCommand(
    Guid LoteId,
    DateTime Fecha,
    decimal? Temperatura = null,
    decimal? Humedad = null,
    decimal? ConsumoAgua = null,
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUserContext;

    public RegistrarBienestarCommandHandler(
        IRegistroBienestarRepository bienestarRepository,
        ILoteRepository loteRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserContext currentUserContext)
    {
        _bienestarRepository = bienestarRepository;
        _loteRepository = loteRepository;
        _unitOfWork = unitOfWork;
        _currentUserContext = currentUserContext;
    }

    public async Task<Guid> Handle(RegistrarBienestarCommand request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null)
            throw new Exception("Lote no encontrado.");

        var usuarioId = _currentUserContext.UsuarioId ?? Guid.Empty;

        var registroExistente = await _bienestarRepository.ObtenerPorLoteYFechaAsync(request.LoteId, request.Fecha);

        if (registroExistente != null)
        {
            registroExistente.Actualizar(
                request.Temperatura ?? registroExistente.Temperatura,
                request.Humedad ?? registroExistente.Humedad,
                request.ConsumoAgua ?? registroExistente.ConsumoAgua,
                request.Observaciones ?? registroExistente.Observaciones);
            
            _bienestarRepository.Actualizar(registroExistente);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return registroExistente.Id;
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
                usuarioId);

            _bienestarRepository.Agregar(nuevoRegistro);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return nuevoRegistro.Id;
        }
    }
}
