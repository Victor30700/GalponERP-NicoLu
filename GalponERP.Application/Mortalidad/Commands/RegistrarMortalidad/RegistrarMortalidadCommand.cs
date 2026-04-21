using GalponERP.Application.Interfaces;
using System.Text.Json.Serialization;
using MediatR;

namespace GalponERP.Application.Mortalidad.Commands.RegistrarMortalidad;

public record RegistrarMortalidadCommand(
    Guid LoteId,
    int Cantidad,
    string Causa,
    DateTime Fecha) : IRequest<Guid>, IAuditableCommand
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
