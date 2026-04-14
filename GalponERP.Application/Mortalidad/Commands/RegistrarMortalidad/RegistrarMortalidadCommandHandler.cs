using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Mortalidad.Commands.RegistrarMortalidad;

public class RegistrarMortalidadCommandHandler : IRequestHandler<RegistrarMortalidadCommand, Guid>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IMortalidadRepository _mortalidadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarMortalidadCommandHandler(
        ILoteRepository loteRepository,
        IMortalidadRepository mortalidadRepository,
        IUnitOfWork unitOfWork)
    {
        _loteRepository = loteRepository;
        _mortalidadRepository = mortalidadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegistrarMortalidadCommand request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);

        if (lote == null)
        {
            throw new Exception("Lote no encontrado.");
        }

        // 1. Registrar las bajas en la entidad Lote (actualiza contadores)
        lote.RegistrarBajas(request.Cantidad);

        // Asegurar que la fecha sea UTC para PostgreSQL
        var fechaUtc = request.Fecha.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(request.Fecha, DateTimeKind.Utc) 
            : request.Fecha.ToUniversalTime();

        // 2. Crear el registro de mortalidad diaria
        var mortalidad = new MortalidadDiaria(
            Guid.NewGuid(),
            request.LoteId,
            fechaUtc,
            request.Cantidad,
            request.Causa,
            request.UsuarioId
        );

        // 3. Persistir
        _mortalidadRepository.Agregar(mortalidad);
        _loteRepository.Actualizar(lote);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return mortalidad.Id;
    }
}
