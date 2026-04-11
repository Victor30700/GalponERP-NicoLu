using System.Text.Json.Serialization;
using MediatR;

namespace GalponERP.Application.Pesajes.Commands.ActualizarPesaje;

public record ActualizarPesajeCommand(
    Guid Id,
    DateTime Fecha,
    decimal PesoPromedioGramos,
    int CantidadMuestreada) : IRequest<Unit>
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
