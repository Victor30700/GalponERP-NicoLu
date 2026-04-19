namespace GalponERP.Application.Nutricion.Formulas.Commands.CrearFormula;

public record FormulaDetalleDto(Guid ProductoId, decimal CantidadPorBase);

public record CrearFormulaCommand(
    string Nombre,
    string Etapa,
    decimal CantidadBase,
    List<FormulaDetalleDto> Detalles) : MediatR.IRequest<Guid>;
