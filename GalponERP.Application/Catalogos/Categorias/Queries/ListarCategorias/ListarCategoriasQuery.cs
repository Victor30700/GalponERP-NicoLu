using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Catalogos.Categorias.Queries.ListarCategorias;

public record ListarCategoriasQuery() : IRequest<IEnumerable<CategoriaResponse>>;

public record CategoriaResponse(Guid Id, string Nombre, string? Descripcion);

public class ListarCategoriasQueryHandler : IRequestHandler<ListarCategoriasQuery, IEnumerable<CategoriaResponse>>
{
    private readonly ICategoriaProductoRepository _categoriaRepository;

    public ListarCategoriasQueryHandler(ICategoriaProductoRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<IEnumerable<CategoriaResponse>> Handle(ListarCategoriasQuery request, CancellationToken cancellationToken)
    {
        var categorias = await _categoriaRepository.ObtenerTodasAsync();
        return categorias
            .Where(c => c.IsActive)
            .Select(c => new CategoriaResponse(c.Id, c.Nombre, c.Descripcion));
    }
}
