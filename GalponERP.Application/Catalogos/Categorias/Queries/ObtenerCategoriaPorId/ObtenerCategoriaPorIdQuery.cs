using GalponERP.Domain.Interfaces.Repositories;
using MediatR;
using GalponERP.Application.Catalogos.Categorias.Queries.ListarCategorias;

namespace GalponERP.Application.Catalogos.Categorias.Queries.ObtenerCategoriaPorId;

public record ObtenerCategoriaPorIdQuery(Guid Id) : IRequest<CategoriaResponse?>;

public class ObtenerCategoriaPorIdQueryHandler : IRequestHandler<ObtenerCategoriaPorIdQuery, CategoriaResponse?>
{
    private readonly ICategoriaProductoRepository _categoriaRepository;

    public ObtenerCategoriaPorIdQueryHandler(ICategoriaProductoRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<CategoriaResponse?> Handle(ObtenerCategoriaPorIdQuery request, CancellationToken cancellationToken)
    {
        var categoria = await _categoriaRepository.ObtenerPorIdAsync(request.Id);
        
        if (categoria == null || !categoria.IsActive)
            return null;

        return new CategoriaResponse(categoria.Id, categoria.Nombre, categoria.Descripcion);
    }
}
