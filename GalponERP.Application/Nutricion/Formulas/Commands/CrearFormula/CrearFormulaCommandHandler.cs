using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Domain.Exceptions;
using MediatR;

namespace GalponERP.Application.Nutricion.Formulas.Commands.CrearFormula;

public class CrearFormulaCommandHandler : IRequestHandler<CrearFormulaCommand, Guid>
{
    private readonly IFormulaRepository _formulaRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearFormulaCommandHandler(
        IFormulaRepository formulaRepository, 
        IProductoRepository productoRepository,
        IUnitOfWork unitOfWork)
    {
        _formulaRepository = formulaRepository;
        _productoRepository = productoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CrearFormulaCommand request, CancellationToken cancellationToken)
    {
        var formula = new Formula(
            Guid.NewGuid(),
            request.Nombre,
            request.Etapa,
            request.CantidadBase
        );

        foreach (var detalle in request.Detalles)
        {
            var producto = await _productoRepository.ObtenerPorIdAsync(detalle.ProductoId);
            if (producto == null)
            {
                throw new FormulaDomainException($"El producto con ID {detalle.ProductoId} no existe. No se puede crear la fórmula con ingredientes inexistentes.");
            }

            formula.AgregarDetalle(detalle.ProductoId, detalle.CantidadPorBase);
        }

        _formulaRepository.Agregar(formula);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return formula.Id;
    }
}
