using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Ventas.Commands.AnularPago;

public record AnularPagoVentaCommand(Guid VentaId, Guid PagoId, Guid UsuarioId) : IRequest, IAuditableCommand;
