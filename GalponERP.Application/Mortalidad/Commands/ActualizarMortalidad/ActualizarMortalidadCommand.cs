using System.Text.Json.Serialization;
using MediatR;

namespace GalponERP.Application.Mortalidad.Commands.ActualizarMortalidad;

public record ActualizarMortalidadCommand(
    Guid Id,
    int Cantidad,
    string Causa,
    DateTime Fecha) : IRequest<Unit>
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
