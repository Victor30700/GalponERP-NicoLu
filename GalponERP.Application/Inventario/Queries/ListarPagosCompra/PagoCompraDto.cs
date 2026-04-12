using GalponERP.Domain.Entities;

namespace GalponERP.Application.Inventario.Queries.ListarPagosCompra;

public record PagoCompraDto(
    Guid Id,
    Guid CompraId,
    decimal Monto,
    DateTime FechaPago,
    MetodoPago MetodoPago,
    Guid UsuarioId);
