using MediatR;

namespace GalponERP.Application.Nutricion.Formulas.Queries.GetFormulaById;

public record GetFormulaByIdQuery(Guid Id) : IRequest<FormulaResponse?>;
