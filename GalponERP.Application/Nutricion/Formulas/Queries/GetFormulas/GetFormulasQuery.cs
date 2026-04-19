using MediatR;

namespace GalponERP.Application.Nutricion.Formulas.Queries.GetFormulas;

public record GetFormulasQuery() : IRequest<IEnumerable<FormulaResponse>>;
