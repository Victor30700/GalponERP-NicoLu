using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Sanidad.Commands.ActualizarBienestar;

public record ActualizarBienestarCommand(
    Guid Id,
    DateTime Fecha,
    decimal Temperatura,
    decimal Humedad,
    decimal ConsumoAgua,
    string? Observaciones,
    Guid UsuarioId,
    string? Version = null) : IRequest, IAuditableCommand;
