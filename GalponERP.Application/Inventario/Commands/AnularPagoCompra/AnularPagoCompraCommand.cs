using MediatR;

namespace GalponERP.Application.Inventario.Commands.AnularPagoCompra;

public record AnularPagoCompraCommand(Guid CompraId, Guid PagoId, Guid UsuarioId) : IRequest;
