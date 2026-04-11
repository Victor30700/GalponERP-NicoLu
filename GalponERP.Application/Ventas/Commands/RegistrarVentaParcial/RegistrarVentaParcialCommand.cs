using MediatR;

namespace GalponERP.Application.Ventas.Commands.RegistrarVentaParcial;

/// <summary>
/// Comando para registrar una venta parcial de pollos de un lote.
/// </summary>
/// <param name="LoteId">ID del lote del que se venden los pollos.</param>
/// <param name="ClienteId">ID del cliente que realiza la compra.</param>
/// <param name="Fecha">Fecha de la venta.</param>
/// <param name="CantidadPollos">Cantidad de pollos vendidos.</param>
/// <param name="PesoTotalVendido">Peso total de la venta en Kg.</param>
/// <param name="PrecioPorKilo">Precio por cada kilo vendido.</param>
public record RegistrarVentaParcialCommand(
    Guid LoteId,
    Guid ClienteId,
    DateTime Fecha,
    int CantidadPollos,
    decimal PesoTotalVendido,
    decimal PrecioPorKilo) : IRequest<Guid>;
