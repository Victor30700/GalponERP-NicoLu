namespace GalponERP.Application.Ventas.Queries.ObtenerPagosPorVenta;

public record PagoResponse(
    Guid Id,
    Guid VentaId,
    decimal Monto,
    DateTime FechaPago,
    string MetodoPago,
    Guid UsuarioId,
    bool IsActive
);
