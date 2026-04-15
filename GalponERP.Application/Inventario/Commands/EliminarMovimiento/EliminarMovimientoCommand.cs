using MediatR;

namespace GalponERP.Application.Inventario.Commands.EliminarMovimiento;

public record EliminarMovimientoCommand(Guid Id, Guid UsuarioId) : IRequest;
