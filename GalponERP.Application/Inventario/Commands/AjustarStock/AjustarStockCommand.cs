using MediatR;
using System.Text.Json.Serialization;

namespace GalponERP.Application.Inventario.Commands;

public record AjustarStockCommand(
    Guid ProductoId,
    decimal CantidadFisica,
    string? Nota = null) : IRequest<Guid>
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
