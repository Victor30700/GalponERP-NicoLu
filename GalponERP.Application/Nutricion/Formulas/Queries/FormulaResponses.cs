namespace GalponERP.Application.Nutricion.Formulas.Queries;

public record FormulaResponse(
    Guid Id,
    string Nombre,
    string Etapa,
    decimal CantidadBase,
    bool IsActive,
    List<FormulaDetalleResponse> Detalles);

public record FormulaDetalleResponse(
    Guid Id,
    Guid ProductoId,
    string ProductoNombre,
    decimal CantidadPorBase);
