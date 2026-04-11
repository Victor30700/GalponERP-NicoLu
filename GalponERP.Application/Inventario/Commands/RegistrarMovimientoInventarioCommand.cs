using System.Text.Json.Serialization;
using GalponERP.Domain.Entities;
using MediatR;

namespace GalponERP.Application.Inventario.Commands.RegistrarMovimiento;

public record RegistrarMovimientoInventarioCommand(
    Guid ProductoId,
    Guid? LoteId,
    decimal Cantidad,
    TipoMovimiento Tipo,
    DateTime Fecha,
    string? Justificacion = null) : IRequest<Guid>
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
