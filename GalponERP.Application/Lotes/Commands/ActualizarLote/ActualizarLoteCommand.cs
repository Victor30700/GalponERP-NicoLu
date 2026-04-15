using System.Text.Json.Serialization;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Lotes.Commands.ActualizarLote;

public record ActualizarLoteCommand(
    Guid Id,
    string Nombre,
    Guid GalponId,
    DateTime FechaIngreso,
    int CantidadInicial,
    decimal CostoUnitarioPollito) : IRequest<Unit>
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
