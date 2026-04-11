using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Mortalidad.Commands.EliminarMortalidad;

public class EliminarMortalidadCommandHandler : IRequestHandler<EliminarMortalidadCommand, Unit>
{
    private readonly IMortalidadRepository _mortalidadRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarMortalidadCommandHandler(
        IMortalidadRepository mortalidadRepository,
        ILoteRepository loteRepository,
        IUnitOfWork unitOfWork)
    {
        _mortalidadRepository = mortalidadRepository;
        _loteRepository = loteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(EliminarMortalidadCommand request, CancellationToken cancellationToken)
    {
        var mortalidad = await _mortalidadRepository.ObtenerPorIdAsync(request.Id);

        if (mortalidad == null)
        {
            throw new Exception("Registro de mortalidad no encontrado.");
        }

        var lote = await _loteRepository.ObtenerPorIdAsync(mortalidad.LoteId);
        if (lote == null)
        {
            throw new Exception("Lote no encontrado.");
        }

        // 1. Revertir contadores en el lote
        lote.EliminarMortalidad(mortalidad.CantidadBajas);

        // 2. Soft Delete del registro
        mortalidad.Eliminar();
        mortalidad.SetAuditoriaModificacion(DateTime.UtcNow, request.UsuarioId);

        // 3. Persistir
        _mortalidadRepository.Actualizar(mortalidad);
        _loteRepository.Actualizar(lote);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
