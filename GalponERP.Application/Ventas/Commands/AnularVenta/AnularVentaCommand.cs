using MediatR;

namespace GalponERP.Application.Ventas.Commands.AnularVenta;

public record AnularVentaCommand(Guid VentaId, Guid UsuarioId) : IRequest<Unit>;
