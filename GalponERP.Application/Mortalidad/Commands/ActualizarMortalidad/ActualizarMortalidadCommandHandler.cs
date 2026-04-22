using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.Exceptions;
using MediatR;

namespace GalponERP.Application.Mortalidad.Commands.ActualizarMortalidad;

public class ActualizarMortalidadCommandHandler : IRequestHandler<ActualizarMortalidadCommand, Unit>
{
    private readonly IMortalidadRepository _mortalidadRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarMortalidadCommandHandler(
        IMortalidadRepository mortalidadRepository,
        ILoteRepository loteRepository,
        IUnitOfWork unitOfWork)
    {
        _mortalidadRepository = mortalidadRepository;
        _loteRepository = loteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ActualizarMortalidadCommand request, CancellationToken cancellationToken)
    {
        var mortalidad = await _mortalidadRepository.ObtenerPorIdAsync(request.Id);

        if (mortalidad == null)
        {
            throw new Exception("Registro de mortalidad no encontrado.");
        }

        if (mortalidad.Version.ToString() != request.Version)
        {
            throw new ConcurrencyException();
        }

        var lote = await _loteRepository.ObtenerPorIdAsync(mortalidad.LoteId);
        if (lote == null)
        {
            throw new Exception("Lote no encontrado.");
        }

        // 1. Corregir contadores en el lote
        lote.CorregirMortalidad(mortalidad.CantidadBajas, request.Cantidad);

        // 2. Actualizar el registro de mortalidad
        mortalidad.Actualizar(request.Fecha, request.Cantidad, request.Causa);
        mortalidad.SetAuditoriaModificacion(DateTime.UtcNow, request.UsuarioId);

        // 3. Persistir
        _mortalidadRepository.Actualizar(mortalidad);
        _loteRepository.Actualizar(lote);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
