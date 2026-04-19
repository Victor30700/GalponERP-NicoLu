using GalponERP.Application.Interfaces;
using GalponERP.Domain.Exceptions;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Nutricion.Formulas.Commands.EliminarFormula;

public class EliminarFormulaCommandHandler : IRequestHandler<EliminarFormulaCommand, Unit>
{
    private readonly IFormulaRepository _formulaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarFormulaCommandHandler(IFormulaRepository formulaRepository, IUnitOfWork unitOfWork)
    {
        _formulaRepository = formulaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(EliminarFormulaCommand request, CancellationToken cancellationToken)
    {
        var formula = await _formulaRepository.ObtenerPorIdAsync(request.Id);
        if (formula == null)
            throw new FormulaDomainException("La fórmula no existe.");

        formula.Desactivar();
        _formulaRepository.Actualizar(formula);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
