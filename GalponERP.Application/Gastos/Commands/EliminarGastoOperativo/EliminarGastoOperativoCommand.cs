using GalponERP.Application.Interfaces;
using System.Text.Json.Serialization;
using MediatR;

namespace GalponERP.Application.Gastos.Commands.EliminarGastoOperativo;

public record EliminarGastoOperativoCommand(Guid Id) : IRequest<Unit>, IAuditableCommand
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
