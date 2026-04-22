using GalponERP.Application.Interfaces;
using GalponERP.Domain.Exceptions;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.Exceptions;
using MediatR;

namespace GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula;

public class ActualizarFormulaCommandHandler : IRequestHandler<ActualizarFormulaCommand, Unit>
{
    private readonly IFormulaRepository _formulaRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarFormulaCommandHandler(
        IFormulaRepository formulaRepository, 
        IProductoRepository productoRepository,
        IUnitOfWork unitOfWork)
    {
        _formulaRepository = formulaRepository;
        _productoRepository = productoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ActualizarFormulaCommand request, CancellationToken cancellationToken)
    {
        var formula = await _formulaRepository.ObtenerPorIdAsync(request.Id);
        if (formula == null)
            throw new FormulaDomainException($"La fórmula con ID {request.Id} no existe.");

        if (formula.Version.ToString() != request.Version)
        {
            throw new ConcurrencyException();
        }

        formula.Actualizar(request.Nombre, request.Etapa, request.CantidadBase);
        
        // Obtener los IDs de productos actuales ACTIVOS en la fórmula
        var productosActualesActivos = formula.Detalles
            .Where(d => d.IsActive)
            .Select(d => d.ProductoId)
            .ToList();
        
        var productosNuevos = request.Detalles.Select(d => d.ProductoId).ToList();

        // 1. Eliminar (soft delete) detalles que ya no están en la petición
        foreach (var productoId in productosActualesActivos.Where(id => !productosNuevos.Contains(id)))
        {
            formula.EliminarDetalle(productoId);
        }

        // 2. Actualizar o añadir nuevos detalles
        foreach (var detalleDto in request.Detalles)
        {
            if (productosActualesActivos.Contains(detalleDto.ProductoId))
            {
                // Actualizar existente
                formula.ActualizarDetalle(detalleDto.ProductoId, detalleDto.CantidadPorBase);
            }
            else
            {
                // Validar que el producto existe antes de añadirlo
                var producto = await _productoRepository.ObtenerPorIdAsync(detalleDto.ProductoId);
                if (producto == null)
                {
                    throw new FormulaDomainException($"El ingrediente con ID {detalleDto.ProductoId} no existe. Por favor verifique el inventario.");
                }

                // Añadir nuevo
                formula.AgregarDetalle(detalleDto.ProductoId, detalleDto.CantidadPorBase);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
