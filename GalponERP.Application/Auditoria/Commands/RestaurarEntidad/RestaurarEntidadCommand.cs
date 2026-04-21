using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Primitives;
using MediatR;

namespace GalponERP.Application.Auditoria.Commands.RestaurarEntidad;

public record RestaurarEntidadCommand(string Entidad, Guid Id) : IRequest, IAuditableCommand;

public class RestaurarEntidadCommandHandler : IRequestHandler<RestaurarEntidadCommand>
{
    private readonly IGalponDbContext _context;

    public RestaurarEntidadCommandHandler(IGalponDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RestaurarEntidadCommand request, CancellationToken cancellationToken)
    {
        Entity? entidad = request.Entidad.ToLower() switch
        {
            "lote" => await _context.ObtenerEntidadPorIdAsync<Lote>(request.Id, cancellationToken),
            "gasto" or "gastooperativo" => await _context.ObtenerEntidadPorIdAsync<GastoOperativo>(request.Id, cancellationToken),
            "venta" => await _context.ObtenerEntidadPorIdAsync<Venta>(request.Id, cancellationToken),
            "mortalidad" or "mortalidaddiaria" => await _context.ObtenerEntidadPorIdAsync<MortalidadDiaria>(request.Id, cancellationToken),
            "pesaje" or "pesajelote" => await _context.ObtenerEntidadPorIdAsync<PesajeLote>(request.Id, cancellationToken),
            "producto" => await _context.ObtenerEntidadPorIdAsync<Producto>(request.Id, cancellationToken),
            "cliente" => await _context.ObtenerEntidadPorIdAsync<Cliente>(request.Id, cancellationToken),
            "proveedor" => await _context.ObtenerEntidadPorIdAsync<Proveedor>(request.Id, cancellationToken),
            _ => throw new ArgumentException($"La entidad '{request.Entidad}' no es válida para restauración.")
        };

        if (entidad == null)
        {
            throw new KeyNotFoundException($"No se encontró la entidad {request.Entidad} con ID {request.Id}");
        }

        // Restaurar vía reflexión ya que IsActive tiene un private set en la clase base Entity.
        var prop = typeof(Entity).GetProperty("IsActive");
        prop?.SetValue(entidad, true);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
