using GalponERP.Application.Interfaces;
using GalponERP.Domain.Exceptions;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula;

public class ActualizarFormulaCommandHandler : IRequestHandler<ActualizarFormulaCommand, Unit>
{
    private readonly IFormulaRepository _formulaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarFormulaCommandHandler(IFormulaRepository formulaRepository, IUnitOfWork unitOfWork)
    {
        _formulaRepository = formulaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ActualizarFormulaCommand request, CancellationToken cancellationToken)
    {
        var formula = await _formulaRepository.ObtenerPorIdAsync(request.Id);
        if (formula == null)
            throw new FormulaDomainException("La fórmula no existe.");

        formula.Actualizar(request.Nombre, request.Etapa, request.CantidadBase);
        
        formula.LimpiarDetalles();
        foreach (var detalle in request.Detalles)
        {
            formula.AgregarDetalle(detalle.ProductoId, detalle.CantidadPorBase);
        }

        _formulaRepository.Actualizar(formula);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
