using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Catalogos.UnidadesMedida.Queries.ListarUnidadesMedida;

public record ListarUnidadesMedidaQuery() : IRequest<IEnumerable<UnidadMedidaResponse>>;

public record UnidadMedidaResponse(Guid Id, string Nombre, string Abreviatura);

public class ListarUnidadesMedidaQueryHandler : IRequestHandler<ListarUnidadesMedidaQuery, IEnumerable<UnidadMedidaResponse>>
{
    private readonly IUnidadMedidaRepository _unidadMedidaRepository;

    public ListarUnidadesMedidaQueryHandler(IUnidadMedidaRepository unidadMedidaRepository)
    {
        _unidadMedidaRepository = unidadMedidaRepository;
    }

    public async Task<IEnumerable<UnidadMedidaResponse>> Handle(ListarUnidadesMedidaQuery request, CancellationToken cancellationToken)
    {
        var unidades = await _unidadMedidaRepository.ObtenerTodasAsync();
        return unidades
            .Where(u => u.IsActive)
            .Select(u => new UnidadMedidaResponse(u.Id, u.Nombre, u.Abreviatura));
    }
}
