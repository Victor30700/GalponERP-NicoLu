using MediatR;

namespace GalponERP.Application.Productos.Queries;

public record ProductoResponse(
    Guid Id,
    string Nombre,
    Guid CategoriaId,
    string CategoriaNombre,
    Guid UnidadMedidaId,
    string UnidadMedidaNombre,
    decimal EquivalenciaEnKg,
    decimal UmbralMinimo,
    bool IsActive);
