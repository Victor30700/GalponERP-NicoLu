using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Nutricion.Formulas.Commands.RegistrarConsumoFormula;

public record RegistrarConsumoFormulaCommand(
    Guid LoteId,
    Guid FormulaId,
    decimal CantidadTotalPreparada,
    DateTime Fecha,
    string? Justificacion) : IRequest<Guid>, IAuditableCommand;
