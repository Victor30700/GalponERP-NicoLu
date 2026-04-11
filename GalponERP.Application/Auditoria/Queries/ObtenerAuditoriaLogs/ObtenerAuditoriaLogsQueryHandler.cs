using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Auditoria.Queries.ObtenerAuditoriaLogs;

public class ObtenerAuditoriaLogsQueryHandler : IRequestHandler<ObtenerAuditoriaLogsQuery, IEnumerable<AuditoriaLog>>
{
    private readonly IAuditoriaRepository _auditoriaRepository;

    public ObtenerAuditoriaLogsQueryHandler(IAuditoriaRepository auditoriaRepository)
    {
        _auditoriaRepository = auditoriaRepository;
    }

    public async Task<IEnumerable<AuditoriaLog>> Handle(ObtenerAuditoriaLogsQuery request, CancellationToken cancellationToken)
    {
        return await _auditoriaRepository.ObtenerTodosAsync();
    }
}
