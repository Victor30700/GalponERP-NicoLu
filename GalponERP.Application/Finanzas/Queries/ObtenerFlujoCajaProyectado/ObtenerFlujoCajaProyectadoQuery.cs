using MediatR;

namespace GalponERP.Application.Finanzas.Queries.ObtenerFlujoCajaProyectado;

public record ObtenerFlujoCajaProyectadoQuery() : IRequest<FlujoCajaProyectadoResponse>;

public record FlujoCajaProyectadoResponse(
    decimal SaldoActual,
    decimal TotalCuentasPorCobrar,
    decimal TotalCuentasPorPagar,
    decimal CostoProyectadoAlimento30Dias,
    decimal FlujoNetoProyectado30Dias,
    IEnumerable<DetalleProyeccionDto> Detalle);

public record DetalleProyeccionDto(
    string Concepto,
    decimal Monto,
    string Tipo, // Ingreso, Egreso, Proyeccion
    DateTime? FechaEstimada);
