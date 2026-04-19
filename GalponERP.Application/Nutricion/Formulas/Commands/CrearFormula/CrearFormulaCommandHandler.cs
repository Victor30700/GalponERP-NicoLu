using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Nutricion.Formulas.Commands.CrearFormula;

public class CrearFormulaCommandHandler : IRequestHandler<CrearFormulaCommand, Guid>
{
    private readonly IFormulaRepository _formulaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearFormulaCommandHandler(IFormulaRepository formulaRepository, IUnitOfWork unitOfWork)
    {
        _formulaRepository = formulaRepository;
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
            formula.AgregarDetalle(detalle.ProductoId, detalle.CantidadPorBase);
        }

        _formulaRepository.Agregar(formula);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return formula.Id;
    }
}
