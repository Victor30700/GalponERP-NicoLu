using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Nutricion.Formulas.Commands.EliminarFormula;

public record EliminarFormulaCommand(Guid Id) : IRequest<Unit>, IAuditableCommand;
