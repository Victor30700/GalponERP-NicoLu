using System.Text.Json.Serialization;
using MediatR;

namespace GalponERP.Application.Pesajes.Commands.EliminarPesaje;

public record EliminarPesajeCommand(Guid Id) : IRequest<Unit>
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
