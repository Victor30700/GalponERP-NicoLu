using System.Text.Json.Serialization;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Lotes.Commands.ActualizarLote;

public record ActualizarLoteCommand(
    Guid Id,
    string Nombre,
    Guid GalponId,
    DateTime FechaIngreso,
    int CantidadInicial,
    decimal CostoUnitarioPollito,
    string? Version = null) : IRequest<Unit>, IAuditableCommand
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
