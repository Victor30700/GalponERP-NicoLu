using System.Text.Json.Serialization;
using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Ventas.Commands.ActualizarVenta;

public record ActualizarVentaCommand(
    Guid Id,
    int CantidadPollos,
    decimal PesoTotalVendido,
    decimal PrecioPorKilo,
    string? Version = null) : IRequest, IAuditableCommand
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
