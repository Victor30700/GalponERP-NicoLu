using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using GalponERP.Application.Hubs;

namespace GalponERP.Application.Pesajes.Commands.RegistrarPesaje;

public class RegistrarPesajeCommandHandler : IRequestHandler<RegistrarPesajeCommand, Guid>
{
    private readonly IPesajeLoteRepository _pesajeRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<NotificationHub> _hubContext;

    public RegistrarPesajeCommandHandler(
        IPesajeLoteRepository pesajeRepository,
        ILoteRepository loteRepository,
        IUnitOfWork unitOfWork,
        IHubContext<NotificationHub> hubContext)
    {
        _pesajeRepository = pesajeRepository;
        _loteRepository = loteRepository;
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
    }

    public async Task<Guid> Handle(RegistrarPesajeCommand request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null)
        {
            throw new Exception("El lote no existe.");
        }

        var fechaUtc = request.Fecha.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(request.Fecha, DateTimeKind.Utc) 
            : request.Fecha.ToUniversalTime();

        var pesaje = new PesajeLote(
            Guid.NewGuid(),
            request.LoteId,
            fechaUtc,
            request.PesoPromedioGramos,
            request.CantidadMuestreada,
            request.UsuarioId);

        _pesajeRepository.Agregar(pesaje);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Lote", "LoteActualizado", cancellationToken);

        return pesaje.Id;
    }
}
