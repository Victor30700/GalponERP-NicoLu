using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Pesajes.Commands.RegistrarPesaje;

public class RegistrarPesajeCommandHandler : IRequestHandler<RegistrarPesajeCommand, Guid>
{
    private readonly IPesajeLoteRepository _pesajeRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarPesajeCommandHandler(
        IPesajeLoteRepository pesajeRepository,
        ILoteRepository loteRepository,
        IUnitOfWork unitOfWork)
    {
        _pesajeRepository = pesajeRepository;
        _loteRepository = loteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegistrarPesajeCommand request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null)
        {
            throw new Exception("El lote no existe.");
        }

        var pesaje = new PesajeLote(
            Guid.NewGuid(),
            request.LoteId,
            request.Fecha,
            request.PesoPromedioGramos,
            request.CantidadMuestreada);

        _pesajeRepository.Agregar(pesaje);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return pesaje.Id;
    }
}
