using System.Text.Json.Serialization;
using MediatR;

namespace GalponERP.Application.Mortalidad.Commands.EliminarMortalidad;

public record EliminarMortalidadCommand(Guid Id) : IRequest<Unit>
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
