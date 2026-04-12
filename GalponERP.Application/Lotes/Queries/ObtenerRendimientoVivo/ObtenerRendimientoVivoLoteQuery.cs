using MediatR;

namespace GalponERP.Application.Lotes.Queries.ObtenerRendimientoVivo;

public record ObtenerRendimientoVivoLoteQuery(Guid LoteId) : IRequest<RendimientoVivoResponse>;

public record RendimientoVivoResponse(
    Guid LoteId,
    int DiasDeVida,
    decimal PesoPromedioActualGramos,
    decimal BiomasaTotalKg,
    decimal AlimentoConsumidoKg,
    decimal FCRProyectado,
    decimal CostoAlimentoAcumulado,
    decimal CostoPollitos,
    decimal GastosOperativosAcumulados,
    decimal CostoTotalInvertido,
    decimal CostoPorKiloVivo);
