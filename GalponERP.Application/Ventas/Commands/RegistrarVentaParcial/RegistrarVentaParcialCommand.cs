using System.Text.Json.Serialization;
using MediatR;

namespace GalponERP.Application.Ventas.Commands.RegistrarVentaParcial;

/// <summary>
/// Comando para registrar una venta parcial de pollos de un lote.
/// </summary>
public record RegistrarVentaParcialCommand(
    Guid LoteId,
    Guid ClienteId,
    DateTime Fecha,
    int CantidadPollos,
    decimal PesoTotalVendido,
    decimal PrecioPorKilo) : IRequest<Guid>
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
