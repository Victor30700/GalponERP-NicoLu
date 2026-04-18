using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Exceptions;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Calendario.Commands.MarcarVacunaAplicada;

public class MarcarVacunaAplicadaCommandHandler : IRequestHandler<MarcarVacunaAplicadaCommand>
{
    private readonly ICalendarioSanitarioRepository _calendarioRepository;
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUserContext;

    public MarcarVacunaAplicadaCommandHandler(
        ICalendarioSanitarioRepository calendarioRepository,
        IInventarioRepository inventarioRepository,
        IProductoRepository productoRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserContext currentUserContext)
    {
        _calendarioRepository = calendarioRepository;
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _unitOfWork = unitOfWork;
        _currentUserContext = currentUserContext;
    }

    public async Task Handle(MarcarVacunaAplicadaCommand request, CancellationToken cancellationToken)
    {
        var actividad = await _calendarioRepository.ObtenerPorIdAsync(request.ActividadId);

        if (actividad == null)
        {
            throw new Exception("Actividad del calendario no encontrada.");
        }

        if (actividad.Estado == EstadoCalendario.Aplicado)
        {
            throw new Exception("Esta actividad ya ha sido marcada como aplicada.");
        }

        if (!actividad.ProductoIdRecomendado.HasValue)
        {
            throw new Exception("La actividad no tiene un producto recomendado asignado.");
        }

        var producto = await _productoRepository.ObtenerPorIdAsync(actividad.ProductoIdRecomendado.Value);
        if (producto == null)
        {
            throw new Exception("El producto recomendado no existe.");
        }

        var usuarioId = _currentUserContext.UsuarioId;
        if (!usuarioId.HasValue || usuarioId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Usuario no identificado para realizar la operación.");
        }

        // 1. Validar Stock
        var stockActual = await _inventarioRepository.ObtenerStockPorProductoIdAsync(actividad.ProductoIdRecomendado.Value);
        if (stockActual < request.CantidadConsumida)
        {
            throw new InventarioDomainException($"Stock insuficiente para el producto. Disponible: {stockActual}, Requerido: {request.CantidadConsumida}");
        }

        // 2. Cambiar estado en CalendarioSanitario
        actividad.MarcarComoAplicado();

        // 3. Generar MovimientoInventario de tipo SALIDA
        var movimiento = new MovimientoInventario(
            Guid.NewGuid(),
            actividad.ProductoIdRecomendado.Value,
            actividad.LoteId,
            request.CantidadConsumida,
            TipoMovimiento.Salida,
            DateTime.UtcNow,
            usuarioId.Value,
            producto.PesoUnitarioKg, // Capturamos el peso actual como HISTÓRICO
            $"Aplicación de vacuna: {actividad.DescripcionTratamiento}"
        );

        _calendarioRepository.Actualizar(actividad);
        _inventarioRepository.RegistrarMovimiento(movimiento);

        // 4. Actualizar el stock en Kg cacheado en el Producto
        producto.ActualizarStock(request.CantidadConsumida, TipoMovimiento.Salida);
        _productoRepository.Actualizar(producto);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
