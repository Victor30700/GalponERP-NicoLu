using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using GalponERP.Domain.Exceptions;
using GalponERP.Application.Interfaces;
using GalponERP.Application.Hubs;
using Microsoft.AspNetCore.SignalR;
using MediatR;

namespace GalponERP.Application.Ventas.Commands.RegistrarVentaParcial;

public class RegistrarVentaParcialCommandHandler : IRequestHandler<RegistrarVentaParcialCommand, Guid>
{
    private readonly IVentaRepository _ventaRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<NotificationHub> _hubContext;

    public RegistrarVentaParcialCommandHandler(
        IVentaRepository ventaRepository,
        ILoteRepository loteRepository,
        IClienteRepository clienteRepository,
        IUnitOfWork unitOfWork,
        IHubContext<NotificationHub> hubContext)
    {
        _ventaRepository = ventaRepository;
        _loteRepository = loteRepository;
        _clienteRepository = clienteRepository;
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
    }

    public async Task<Guid> Handle(RegistrarVentaParcialCommand request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null)
            throw new LoteDomainException($"Lote con ID {request.LoteId} no encontrado.");

        var cliente = await _clienteRepository.ObtenerPorIdAsync(request.ClienteId);
        if (cliente == null)
            throw new LoteDomainException($"Cliente con ID {request.ClienteId} no encontrado.");

        // Regla de negocio encapsulada en la entidad Lote (Blindaje Fase 1 incluido)
        lote.RegistrarVenta(request.CantidadPollos, request.Fecha);

        // Asegurar que la fecha sea UTC para PostgreSQL
        var fechaUtc = request.Fecha.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(request.Fecha, DateTimeKind.Utc) 
            : request.Fecha.ToUniversalTime();

        var ventaId = Guid.NewGuid();
        var venta = new Venta(
            ventaId,
            request.LoteId,
            request.ClienteId,
            fechaUtc,
            request.CantidadPollos,
            request.PesoTotalVendido,
            new Moneda(request.PrecioPorKilo),
            request.UsuarioId);

        _ventaRepository.Agregar(venta);
        _loteRepository.Actualizar(lote);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notificar venta registrada
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Finanzas", "VentaRegistrada", cancellationToken);

        return ventaId;
    }
}
