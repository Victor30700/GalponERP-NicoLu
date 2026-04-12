using MediatR;

namespace GalponERP.Application.Finanzas.Queries.ObtenerGastosPorCategoria;

public record ObtenerGastosPorCategoriaQuery(DateTime Inicio, DateTime Fin) : IRequest<IEnumerable<GastoCategoriaResponse>>;

public record GastoCategoriaResponse(string Categoria, decimal Total);
