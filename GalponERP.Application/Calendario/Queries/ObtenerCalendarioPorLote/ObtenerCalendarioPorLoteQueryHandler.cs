using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Calendario.Queries.ObtenerCalendarioPorLote;

public class ObtenerCalendarioPorLoteQueryHandler : IRequestHandler<ObtenerCalendarioPorLoteQuery, IEnumerable<CalendarioSanitario>>
{
    private readonly ICalendarioSanitarioRepository _calendarioRepository;

    public ObtenerCalendarioPorLoteQueryHandler(ICalendarioSanitarioRepository calendarioRepository)
    {
        _calendarioRepository = calendarioRepository;
    }

    public async Task<IEnumerable<CalendarioSanitario>> Handle(ObtenerCalendarioPorLoteQuery request, CancellationToken cancellationToken)
    {
        return await _calendarioRepository.ObtenerPorLoteIdAsync(request.LoteId);
    }
}
