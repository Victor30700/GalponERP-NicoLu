using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Ventas.Commands.RegistrarVentaParcial;

public class RegistrarVentaParcialCommandHandler : IRequestHandler<RegistrarVentaParcialCommand, Guid>
{
    private readonly IVentaRepository _ventaRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarVentaParcialCommandHandler(
        IVentaRepository ventaRepository,
        ILoteRepository loteRepository,
        IClienteRepository clienteRepository,
        IUnitOfWork unitOfWork)
    {
        _ventaRepository = ventaRepository;
        _loteRepository = loteRepository;
        _clienteRepository = clienteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegistrarVentaParcialCommand request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null)
            throw new Exception($"Lote con ID {request.LoteId} no encontrado.");

        var cliente = await _clienteRepository.ObtenerPorIdAsync(request.ClienteId);
        if (cliente == null)
            throw new Exception($"Cliente con ID {request.ClienteId} no encontrado.");

        // Regla de negocio encapsulada en la entidad Lote
        lote.RegistrarVenta(request.CantidadPollos);

        var ventaId = Guid.NewGuid();
        var venta = new Venta(
            ventaId,
            request.LoteId,
            request.ClienteId,
            request.Fecha,
            request.CantidadPollos,
            new Moneda(request.PrecioUnitario));

        _ventaRepository.Agregar(venta);
        _loteRepository.Actualizar(lote);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ventaId;
    }
}
