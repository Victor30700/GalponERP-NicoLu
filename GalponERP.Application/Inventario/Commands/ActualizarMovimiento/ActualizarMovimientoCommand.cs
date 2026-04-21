using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Inventario.Commands.ActualizarMovimiento;

public record ActualizarMovimientoCommand(
    Guid Id,
    Guid ProductoId,
    decimal Cantidad,
    DateTime Fecha,
    string? Justificacion,
    Guid UsuarioId,
    string? Version = null) : IRequest, IAuditableCommand;
