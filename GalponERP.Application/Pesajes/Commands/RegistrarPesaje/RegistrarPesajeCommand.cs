using GalponERP.Application.Interfaces;
using System.Text.Json.Serialization;
using MediatR;

namespace GalponERP.Application.Pesajes.Commands.RegistrarPesaje;

public record RegistrarPesajeCommand(
    Guid LoteId,
    DateTime Fecha,
    decimal PesoPromedioGramos,
    int CantidadMuestreada) : IRequest<Guid>, IAuditableCommand
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
