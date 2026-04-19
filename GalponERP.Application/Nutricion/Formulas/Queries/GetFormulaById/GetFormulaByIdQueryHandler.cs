using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Nutricion.Formulas.Queries.GetFormulaById;

public class GetFormulaByIdQueryHandler : IRequestHandler<GetFormulaByIdQuery, FormulaResponse?>
{
    private readonly IFormulaRepository _formulaRepository;

    public GetFormulaByIdQueryHandler(IFormulaRepository formulaRepository)
    {
        _formulaRepository = formulaRepository;
    }

    public async Task<FormulaResponse?> Handle(GetFormulaByIdQuery request, CancellationToken cancellationToken)
    {
        var formula = await _formulaRepository.ObtenerPorIdAsync(request.Id);
        if (formula == null) return null;

        return new FormulaResponse(
            formula.Id,
            formula.Nombre,
            formula.Etapa,
            formula.CantidadBase,
            formula.IsActive,
            formula.Detalles.Select(d => new FormulaDetalleResponse(
                d.Id,
                d.ProductoId,
                d.Producto?.Nombre ?? "Producto no cargado",
                d.CantidadPorBase
            )).ToList()
        );
    }
}
