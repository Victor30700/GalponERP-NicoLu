using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.Services;
using GalponERP.Domain.ValueObjects;
using GalponERP.Domain.Exceptions;
using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Lotes.Commands.CerrarLote;

public class CerrarLoteCommandHandler : IRequestHandler<CerrarLoteCommand, CerrarLoteResponse>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IVentaRepository _ventaRepository;
    private readonly IGastoOperativoRepository _gastoRepository;
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly CalculadoraCostosLote _calculadoraCostos;
    private readonly IUnitOfWork _unitOfWork;

    public CerrarLoteCommandHandler(
        ILoteRepository loteRepository,
        IVentaRepository ventaRepository,
        IGastoOperativoRepository gastoRepository,
        IInventarioRepository inventarioRepository,
        IProductoRepository productoRepository,
        CalculadoraCostosLote calculadoraCostos,
        IUnitOfWork unitOfWork)
    {
        _loteRepository = loteRepository;
        _ventaRepository = ventaRepository;
        _gastoRepository = gastoRepository;
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _calculadoraCostos = calculadoraCostos;
        _unitOfWork = unitOfWork;
    }

    public async Task<CerrarLoteResponse> Handle(CerrarLoteCommand request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null)
            throw new KeyNotFoundException($"Lote con ID {request.LoteId} no encontrado.");

        if (lote.Estado == EstadoLote.Cerrado)
            throw new LoteDomainException("El lote ya se encuentra en estado 'Cerrado'. No es necesario cerrarlo nuevamente.");

        // 1. Obtener todas las Ventas asociadas al Lote
        var ventas = (await _ventaRepository.ObtenerPorLoteAsync(request.LoteId)).ToList();

        // VALIDACIÓN SPRINT 48: No se puede cerrar el lote si existen cuentas por cobrar pendientes
        var ventasPendientes = ventas.Where(v => v.SaldoPendiente.Monto > 0).ToList();
        if (ventasPendientes.Any())
        {
            throw new LoteDomainException($"No se puede cerrar el lote porque existen {ventasPendientes.Count} ventas con saldo pendiente por cobrar.");
        }

        // 2. Sumar ingresos
        var totalIngresos = ventas.Aggregate(Moneda.Zero, (acc, next) => acc + next.Total);

        // 2. Obtener gastos operativos asociados
        var gastos = await _gastoRepository.ObtenerPorLoteAsync(request.LoteId);

        // 3. Calcular Costo de Pollitos
        var costoPollitos = lote.CostoUnitarioPollito * lote.CantidadInicial;

        // 4. Calcular Costo de Alimento y Total de Alimento en Kg
        var movimientos = await _inventarioRepository.ObtenerPorLoteIdAsync(request.LoteId);
        var productos = await _productoRepository.ObtenerTodosAsync();
        
        // Identificar productos que son alimento (por nombre de categoría)
        var productosAlimento = productos
            .Where(p => p.Categoria?.Nombre.Equals("Alimento", StringComparison.OrdinalIgnoreCase) == true)
            .ToDictionary(p => p.Id, p => p.EquivalenciaEnKg);

        decimal totalAlimentoConsumidoKg = movimientos
            .Where(m => (m.Tipo == TipoMovimiento.Salida || m.Tipo == TipoMovimiento.AjusteSalida) && productosAlimento.ContainsKey(m.ProductoId))
            .Sum(m => m.Cantidad * productosAlimento[m.ProductoId]);

        // (Usando zero por ahora hasta que se implemente costeo de inventario)
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

        // 8. Calcular FCR y Mortalidad
        decimal pesoTotalVendido = ventas.Sum(v => v.PesoTotalVendido);
        decimal fcr = _calculadoraCostos.CalcularFCR(totalAlimentoConsumidoKg, pesoTotalVendido);
        
        decimal porcentajeMortalidad = lote.CantidadInicial > 0 
            ? Math.Round((decimal)lote.MortalidadAcumulada / lote.CantidadInicial * 100, 2, MidpointRounding.AwayFromZero)
            : 0;

        // 9. Marcar el Lote como 'Cerrado' y guardar snapshots
        lote.CerrarLote(fcr, costoTotal, utilidadNeta, porcentajeMortalidad);

        _loteRepository.Actualizar(lote);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CerrarLoteResponse(
            lote.Id,
            totalIngresos.Monto,
            costoTotal.Monto,
            utilidadNeta.Monto,
            fcr,
            porcentajeMortalidad);
    }
}
