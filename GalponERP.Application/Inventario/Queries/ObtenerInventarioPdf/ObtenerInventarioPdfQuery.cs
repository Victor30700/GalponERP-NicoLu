using MediatR;

namespace GalponERP.Application.Inventario.Queries.ObtenerInventarioPdf;

public record ObtenerInventarioPdfQuery(Guid? CategoriaId) : IRequest<byte[]>;
