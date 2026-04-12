using System.Text.Json.Serialization;
using MediatR;

namespace GalponERP.Application.Ventas.Commands.ActualizarVenta;

public record ActualizarVentaCommand(
    Guid VentaId,
    int CantidadPollos,
    decimal PesoTotalVendido,
    decimal PrecioPorKilo) : IRequest
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
