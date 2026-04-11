using GalponERP.Domain.Interfaces.Repositories;
using MediatR;
using GalponERP.Application.Catalogos.UnidadesMedida.Queries.ListarUnidadesMedida;

namespace GalponERP.Application.Catalogos.UnidadesMedida.Queries.ObtenerUnidadMedidaPorId;

public record ObtenerUnidadMedidaPorIdQuery(Guid Id) : IRequest<UnidadMedidaResponse?>;

public class ObtenerUnidadMedidaPorIdQueryHandler : IRequestHandler<ObtenerUnidadMedidaPorIdQuery, UnidadMedidaResponse?>
{
    private readonly IUnidadMedidaRepository _unidadMedidaRepository;

    public ObtenerUnidadMedidaPorIdQueryHandler(IUnidadMedidaRepository unidadMedidaRepository)
    {
        _unidadMedidaRepository = unidadMedidaRepository;
    }

    public async Task<UnidadMedidaResponse?> Handle(ObtenerUnidadMedidaPorIdQuery request, CancellationToken cancellationToken)
    {
        var unidad = await _unidadMedidaRepository.ObtenerPorIdAsync(request.Id);
        
        if (unidad == null || !unidad.IsActive)
            return null;

        return new UnidadMedidaResponse(unidad.Id, unidad.Nombre, unidad.Abreviatura);
    }
}
