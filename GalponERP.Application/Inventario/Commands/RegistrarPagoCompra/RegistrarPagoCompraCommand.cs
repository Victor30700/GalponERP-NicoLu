using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using MediatR;
using System.Text.Json.Serialization;

namespace GalponERP.Application.Inventario.Commands.RegistrarPagoCompra;

public record RegistrarPagoCompraCommand(
    decimal Monto,
    DateTime FechaPago,
    MetodoPago MetodoPago) : IRequest<Guid>, IAuditableCommand
{
    [JsonIgnore]
    public Guid CompraId { get; set; }

    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
