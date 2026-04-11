using MediatR;

namespace GalponERP.Application.Finanzas.Queries.ObtenerFlujoCajaEmpresarial;

public record ObtenerFlujoCajaEmpresarialQuery(DateTime Inicio, DateTime Fin) : IRequest<FlujoCajaDto>;

public record FlujoCajaDto(
    decimal TotalIngresos,
    decimal TotalEgresos,
    decimal UtilidadNeta,
    List<VentaResumenDto> Ventas,
    List<GastoResumenDto> Gastos);

public record VentaResumenDto(Guid Id, DateTime Fecha, string Lote, decimal Monto);
public record GastoResumenDto(Guid Id, DateTime Fecha, string Descripcion, string Tipo, decimal Monto);
