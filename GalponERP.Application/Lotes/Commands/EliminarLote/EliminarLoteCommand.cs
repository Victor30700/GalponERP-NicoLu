using System.Text.Json.Serialization;
using MediatR;

namespace GalponERP.Application.Lotes.Commands.EliminarLote;

public record EliminarLoteCommand(Guid Id) : IRequest<Unit>
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
