using GalponERP.Domain.Entities;
using MediatR;

namespace GalponERP.Application.Auditoria.Queries.ObtenerAuditoriaLogs;

public record ObtenerAuditoriaLogsQuery(
    DateTime? Desde = null, 
    DateTime? Hasta = null, 
    Guid? UsuarioId = null, 
    string? Entidad = null) : IRequest<IEnumerable<AuditoriaLog>>;
