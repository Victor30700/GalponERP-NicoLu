using GalponERP.Application.Interfaces;
using System.Text.Json.Serialization;
using MediatR;

namespace GalponERP.Application.Inventario.Commands.RegistrarConciliacion;

public record ItemConciliacion(Guid ProductoId, decimal CantidadFisica, string? Nota);

public record RegistrarConciliacionStockCommand(IEnumerable<ItemConciliacion> Items) : IRequest, IAuditableCommand
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
