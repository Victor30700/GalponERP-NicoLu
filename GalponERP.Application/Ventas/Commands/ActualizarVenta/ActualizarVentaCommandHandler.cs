using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Ventas.Commands.ActualizarVenta;

public class ActualizarVentaCommandHandler : IRequestHandler<ActualizarVentaCommand>
{
    private readonly IVentaRepository _ventaRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarVentaCommandHandler(
        IVentaRepository ventaRepository,
        ILoteRepository loteRepository,
        IUnitOfWork unitOfWork)
    {
        _ventaRepository = ventaRepository;
        _loteRepository = loteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ActualizarVentaCommand request, CancellationToken cancellationToken)
    {
        var venta = await _ventaRepository.ObtenerPorIdAsync(request.VentaId);
        if (venta == null)
            throw new Exception($"Venta con ID {request.VentaId} no encontrada.");

        var lote = await _loteRepository.ObtenerPorIdAsync(venta.LoteId);
        if (lote == null)
            throw new Exception($"Lote con ID {venta.LoteId} no encontrado.");

        // Si la cantidad de pollos cambió, actualizar el lote
        if (venta.CantidadPollos != request.CantidadPollos)
        {
            lote.CorregirVenta(venta.CantidadPollos, request.CantidadPollos);
            _loteRepository.Actualizar(lote);
        }

        // Actualizar datos de la venta
        venta.ActualizarDatos(
            request.CantidadPollos,
            request.PesoTotalVendido,
            new Moneda(request.PrecioPorKilo));

        _ventaRepository.Actualizar(venta);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
