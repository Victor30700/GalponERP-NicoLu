using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Catalogos.Categorias.Commands.EliminarCategoria;

public record EliminarCategoriaCommand(Guid Id) : IRequest;

public class EliminarCategoriaCommandHandler : IRequestHandler<EliminarCategoriaCommand>
{
    private readonly ICategoriaProductoRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarCategoriaCommandHandler(ICategoriaProductoRepository categoriaRepository, IUnitOfWork unitOfWork)
    {
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EliminarCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _categoriaRepository.ObtenerPorIdSinFiltroAsync(request.Id);
        if (categoria == null)
            throw new Exception("Categoría no encontrada");

        categoria.Eliminar();
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
