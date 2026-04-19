using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Nutricion.Formulas.Queries.GetFormulas;

public class GetFormulasQueryHandler : IRequestHandler<GetFormulasQuery, IEnumerable<FormulaResponse>>
{
    private readonly IFormulaRepository _formulaRepository;

    public GetFormulasQueryHandler(IFormulaRepository formulaRepository)
    {
        _formulaRepository = formulaRepository;
    }

    public async Task<IEnumerable<FormulaResponse>> Handle(GetFormulasQuery request, CancellationToken cancellationToken)
    {
        var formulas = await _formulaRepository.ObtenerTodasAsync();
        return formulas.Select(f => new FormulaResponse(
            f.Id,
            f.Nombre,
            f.Etapa,
            f.CantidadBase,
            f.IsActive,
            f.Detalles.Select(d => new FormulaDetalleResponse(
                d.Id,
                d.ProductoId,
                d.Producto?.Nombre ?? "Producto no cargado",
                d.CantidadPorBase
            )).ToList()
        ));
    }
}
