using GalponERP.Application.Interfaces;
using System.Text.Json.Serialization;
using GalponERP.Domain.ValueObjects;
using MediatR;

namespace GalponERP.Application.Gastos.Commands.RegistrarGastoOperativo;

public record RegistrarGastoOperativoCommand(
    Guid GalponId,
    Guid? LoteId,
    string Descripcion,
    decimal Monto,
    DateTime Fecha,
    string TipoGasto) : IRequest<Guid>, IAuditableCommand
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
