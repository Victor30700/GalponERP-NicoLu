using MediatR;

namespace GalponERP.Application.Productos.Queries;

public record ProductoResponse(
    Guid Id,
    string Nombre,
    Guid CategoriaId,
    string CategoriaNombre,
    Guid UnidadMedidaId,
    string UnidadMedidaNombre,
    decimal PesoUnitarioKg,
    decimal UmbralMinimo,
    decimal StockActual,
    decimal StockActualKg,
    bool IsActive)
{
    // Alias para el usuario que pidió ver el total como equivalenciaEnKg en el API
    public decimal EquivalenciaEnKg => StockActualKg;
}
