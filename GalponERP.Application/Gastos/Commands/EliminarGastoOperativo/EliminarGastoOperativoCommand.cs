using System.Text.Json.Serialization;
using MediatR;

namespace GalponERP.Application.Gastos.Commands.EliminarGastoOperativo;

public record EliminarGastoOperativoCommand(Guid Id) : IRequest<Unit>
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
