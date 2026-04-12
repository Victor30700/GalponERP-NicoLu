using System.Text.Json.Serialization;
using GalponERP.Domain.Entities;
using MediatR;

namespace GalponERP.Application.Ventas.Commands.RegistrarPago;

public record RegistrarPagoVentaCommand(
    decimal Monto,
    DateTime FechaPago,
    MetodoPago MetodoPago) : IRequest<Guid>
{
    [JsonIgnore]
    public Guid VentaId { get; set; }

    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
