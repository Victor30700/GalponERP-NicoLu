using MediatR;
using Microsoft.EntityFrameworkCore;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;

namespace GalponERP.Application.Agentes.Confirmacion.Queries;

public record ObtenerIntencionPendienteQuery(Guid ConversacionId) : IRequest<IntencionPendiente?>;

public class ObtenerIntencionPendienteQueryHandler : IRequestHandler<ObtenerIntencionPendienteQuery, IntencionPendiente?>
{
    private readonly IGalponDbContext _context;

    public ObtenerIntencionPendienteQueryHandler(IGalponDbContext context)
    {
        _context = context;
    }

    public async Task<IntencionPendiente?> Handle(ObtenerIntencionPendienteQuery request, CancellationToken cancellationToken)
    {
        return await _context.IntencionesPendientes
            .Where(i => i.ConversacionId == request.ConversacionId && !i.Procesada && i.IsActive)
            .OrderByDescending(i => i.FechaCreacion)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
