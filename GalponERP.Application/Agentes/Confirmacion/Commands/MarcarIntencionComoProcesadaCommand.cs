using MediatR;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;

namespace GalponERP.Application.Agentes.Confirmacion.Commands;

public record MarcarIntencionComoProcesadaCommand(Guid IntencionId) : IRequest;

public class MarcarIntencionComoProcesadaCommandHandler : IRequestHandler<MarcarIntencionComoProcesadaCommand>
{
    private readonly IGalponDbContext _context;

    public MarcarIntencionComoProcesadaCommandHandler(IGalponDbContext context)
    {
        _context = context;
    }

    public async Task Handle(MarcarIntencionComoProcesadaCommand request, CancellationToken cancellationToken)
    {
        var intencion = await _context.IntencionesPendientes.FindAsync(request.IntencionId);
        if (intencion != null)
        {
            intencion.MarcarComoProcesada();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
