using MediatR;

namespace GalponERP.Application.Inventario.Commands.ActualizarMovimiento;

public record ActualizarMovimientoCommand(
    Guid Id,
    Guid ProductoId,
    decimal Cantidad,
    DateTime Fecha,
    string? Justificacion,
    Guid UsuarioId) : IRequest;
