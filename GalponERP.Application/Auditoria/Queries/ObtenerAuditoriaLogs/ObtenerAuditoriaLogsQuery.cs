using GalponERP.Domain.Entities;
using MediatR;

namespace GalponERP.Application.Auditoria.Queries.ObtenerAuditoriaLogs;

public record ObtenerAuditoriaLogsQuery() : IRequest<IEnumerable<AuditoriaLog>>;
