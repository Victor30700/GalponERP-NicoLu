using MediatR;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;

namespace GalponERP.Application.Agentes.Confirmacion.Commands;

public record RegistrarIntencionCommand(Guid ConversacionId, string PluginNombre, string FuncionNombre, string ParametrosJson) : IRequest<Guid>;

public class RegistrarIntencionCommandHandler : IRequestHandler<RegistrarIntencionCommand, Guid>
{
    private readonly IGalponDbContext _context;

    public RegistrarIntencionCommandHandler(IGalponDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RegistrarIntencionCommand request, CancellationToken cancellationToken)
    {
        var intencion = new IntencionPendiente(
            Guid.NewGuid(),
            request.ConversacionId,
            request.PluginNombre,
            request.FuncionNombre,
            request.ParametrosJson);

        _context.IntencionesPendientes.Add(intencion);
        await _context.SaveChangesAsync(cancellationToken);

        return intencion.Id;
    }
}
