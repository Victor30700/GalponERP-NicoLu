using GalponERP.Application.Nutricion.Formulas.Commands.CrearFormula;
using MediatR;

namespace GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula;

public record ActualizarFormulaCommand(
    Guid Id,
    string Nombre,
    string Etapa,
    decimal CantidadBase,
    List<FormulaDetalleDto> Detalles) : IRequest<Unit>;
