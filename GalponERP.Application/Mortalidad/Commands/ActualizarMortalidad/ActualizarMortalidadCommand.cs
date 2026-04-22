using GalponERP.Application.Interfaces;
using System.Text.Json.Serialization;
using MediatR;

namespace GalponERP.Application.Mortalidad.Commands.ActualizarMortalidad;

public record ActualizarMortalidadCommand(
    Guid Id,
    int Cantidad,
    string Causa,
    DateTime Fecha,
    string? Version = null) : IRequest<Unit>, IAuditableCommand
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
