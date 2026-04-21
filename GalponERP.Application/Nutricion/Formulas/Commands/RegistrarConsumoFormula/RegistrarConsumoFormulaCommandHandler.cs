using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Exceptions;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Nutricion.Formulas.Commands.RegistrarConsumoFormula;

public class RegistrarConsumoFormulaCommandHandler : IRequestHandler<RegistrarConsumoFormulaCommand, Guid>
{
    private readonly IFormulaRepository _formulaRepository;
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly ICalendarioSanitarioRepository _calendarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUserContext;

    public RegistrarConsumoFormulaCommandHandler(
        IFormulaRepository formulaRepository,
        IInventarioRepository inventarioRepository,
        IProductoRepository productoRepository,
        ILoteRepository loteRepository,
        ICalendarioSanitarioRepository calendarioRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserContext currentUserContext)
    {
        _formulaRepository = formulaRepository;
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _loteRepository = loteRepository;
        _calendarioRepository = calendarioRepository;
        _unitOfWork = unitOfWork;
        _currentUserContext = currentUserContext;
    }

    public async Task<Guid> Handle(RegistrarConsumoFormulaCommand request, CancellationToken cancellationToken)
    {
        var formula = await _formulaRepository.ObtenerPorIdAsync(request.FormulaId);
        if (formula == null)
            throw new FormulaDomainException("La fórmula seleccionada no existe.");

        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null)
            throw new Exception("El lote seleccionado no existe.");

        var usuarioId = _currentUserContext.UsuarioId;
        if (!usuarioId.HasValue || usuarioId == Guid.Empty)
            throw new UnauthorizedAccessException("Usuario no identificado.");

        decimal factor = request.CantidadTotalPreparada / formula.CantidadBase;
        var consumoId = Guid.NewGuid();

        foreach (var detalle in formula.Detalles)
        {
            var producto = detalle.Producto; // Incluido en el repositorio
            decimal cantidadARestar = detalle.CantidadPorBase * factor;
            
            // 1. Validar Stock (cantidadARestar es en la unidad de medida del producto, ej. Kg, L, Unidades)
            var stockDisponible = await _inventarioRepository.ObtenerStockPorProductoIdAsync(producto.Id);
            if (stockDisponible < cantidadARestar)
            {
                throw new InventarioDomainException($"Stock insuficiente para {producto.Nombre}. Disponible: {stockDisponible}, Requerido: {cantidadARestar}");
            }

            // 2. Registrar Movimiento de Inventario
            decimal costoConsumo = cantidadARestar * producto.CostoUnitarioActual;
            var movimiento = new MovimientoInventario(
                Guid.NewGuid(),
                producto.Id,
                lote.Id,
                cantidadARestar,
                TipoMovimiento.Salida,
                request.Fecha.ToUniversalTime(),
                usuarioId.Value,
                producto.PesoUnitarioKg,
                request.Justificacion ?? $"Consumo por fórmula: {formula.Nombre}",
                new Moneda(costoConsumo)
            );

            _inventarioRepository.RegistrarMovimiento(movimiento);

            // 3. Actualizar Stock del Producto
            producto.ActualizarStock(cantidadARestar, TipoMovimiento.Salida);
            _productoRepository.Actualizar(producto);

            // 4. Integración Sanitaria
            if (EsMedicamentoOVacuna(producto))
            {
                int diaAplicacion = (request.Fecha.Date - lote.FechaIngreso.Date).Days + 1;
                var registroSanitario = new CalendarioSanitario(
                    Guid.NewGuid(),
                    lote.Id,
                    diaAplicacion,
                    $"Aplicación via alimento (Fórmula: {formula.Nombre}) - {producto.Nombre}",
                    TipoActividad.Antibiotico,
                    producto.Id,
                    cantidadARestar,
                    false,
                    request.Justificacion
                );
                registroSanitario.MarcarComoAplicado();
                _calendarioRepository.Agregar(registroSanitario);

                // Blindaje Fase 1: Actualizar periodo de retiro en el lote
                lote.RegistrarAplicacionMedica(request.Fecha, producto.PeriodoRetiroDias);
            }
        }

        _loteRepository.Actualizar(lote);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return consumoId;
    }

    private bool EsMedicamentoOVacuna(Producto producto)
    {
        if (producto.Categoria == null) return false;
        
        return producto.Categoria.Tipo == TipoCategoria.Medicamento || 
               producto.Categoria.Tipo == TipoCategoria.Vacuna;
    }
}
