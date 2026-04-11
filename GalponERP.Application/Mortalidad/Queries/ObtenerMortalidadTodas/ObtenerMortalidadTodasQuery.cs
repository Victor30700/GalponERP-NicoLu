using GalponERP.Application.Mortalidad.Queries.ObtenerMortalidadPorLote;
using MediatR;

namespace GalponERP.Application.Mortalidad.Queries.ObtenerMortalidadTodas;

public record ObtenerMortalidadTodasQuery() : IRequest<IEnumerable<MortalidadResponse>>;
