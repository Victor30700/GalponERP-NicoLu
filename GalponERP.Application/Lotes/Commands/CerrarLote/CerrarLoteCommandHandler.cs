using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.Services;
using GalponERP.Domain.ValueObjects;
using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Lotes.Commands.CerrarLote;

public class CerrarLoteCommandHandler : IRequestHandler<CerrarLoteCommand, CerrarLoteResponse>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IVentaRepository _ventaRepository;
    private readonly IGastoOperativoRepository _gastoRepository;
    private readonly IInventarioRepository _inventarioRepository;
    private readonly CalculadoraCostosLote _calculadoraCostos;
    private readonly IUnitOfWork _unitOfWork;

    public CerrarLoteCommandHandler(
        ILoteRepository loteRepository,
        IVentaRepository ventaRepository,
        IGastoOperativoRepository gastoRepository,
        IInventarioRepository inventarioRepository,
        CalculadoraCostosLote calculadoraCostos,
        IUnitOfWork unitOfWork)
    {
        _loteRepository = loteRepository;
        _ventaRepository = ventaRepository;
        _gastoRepository = gastoRepository;
        _inventarioRepository = inventarioRepository;
        _calculadoraCostos = calculadoraCostos;
        _unitOfWork = unitOfWork;
    }

    public async Task<CerrarLoteResponse> Handle(CerrarLoteCommand request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null)
            throw new Exception($"Lote con ID {request.LoteId} no encontrado.");

        if (lote.Estado == EstadoLote.Cerrado)
            throw new Exception("El lote ya está cerrado.");

        // 1. Sumar ingresos de todas las Ventas asociadas al Lote
        var ventas = await _ventaRepository.ObtenerPorLoteAsync(request.LoteId);
        var totalIngresos = ventas.Aggregate(Moneda.Zero, (acc, next) => acc + next.Total);

        // 2. Obtener gastos operativos asociados
        var gastos = await _gastoRepository.ObtenerPorLoteAsync(request.LoteId);

        // 3. Calcular Costo de Pollitos
        var costoPollitos = lote.CostoUnitarioPollito * lote.CantidadInicial;

        // 4. Calcular Costo de Alimento (Usando zero por ahora hasta que se implemente costeo de inventario)
        var costoAlimento = Moneda.Zero;

        // 5. Amortización (Supongamos un valor fijo o cero)
        var amortizacion = Moneda.Zero;

        // 6. Calcular Costo Total usando el Servicio de Dominio
        var costoTotal = _calculadoraCostos.CalcularCostoTotal(
            amortizacion,
            costoPollitos,
            costoAlimento,
            gastos);

        // 7. Calcular Utilidad Neta
        var utilidadNeta = totalIngresos - costoTotal;

        // 8. Marcar el Lote como 'Cerrado'
        lote.CerrarLote();

        _loteRepository.Actualizar(lote);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CerrarLoteResponse(
            lote.Id,
            totalIngresos.Monto,
            costoTotal.Monto,
            utilidadNeta.Monto);
    }
}
