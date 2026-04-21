using GalponERP.Application.Interfaces;
using System.Text.Json.Serialization;
using MediatR;

namespace GalponERP.Application.Gastos.Commands.ActualizarGastoOperativo;

public record ActualizarGastoOperativoCommand(
    Guid Id,
    string Descripcion,
    decimal Monto,
    DateTime Fecha,
    string TipoGasto,
    Guid? LoteId = null) : IRequest<Unit>, IAuditableCommand
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
