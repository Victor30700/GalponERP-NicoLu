using MediatR;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Application.Agentes.Chat.Commands.ActualizarResumen;

public record ActualizarResumenCommand(Guid ConversacionId, string Resumen, int UltimoIndice) : IRequest;

public class ActualizarResumenCommandHandler : IRequestHandler<ActualizarResumenCommand>
{
    private readonly IGalponDbContext _context;

    public ActualizarResumenCommandHandler(IGalponDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ActualizarResumenCommand request, CancellationToken cancellationToken)
    {
        var conversacion = await _context.Conversaciones.FindAsync(request.ConversacionId);
        if (conversacion != null)
        {
            conversacion.ActualizarResumen(request.Resumen, request.UltimoIndice);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
