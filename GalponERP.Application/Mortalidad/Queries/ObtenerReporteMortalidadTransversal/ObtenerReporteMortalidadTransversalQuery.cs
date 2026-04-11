using MediatR;

namespace GalponERP.Application.Mortalidad.Queries.ObtenerReporteMortalidadTransversal;

public record ObtenerReporteMortalidadTransversalQuery(DateTime Inicio, DateTime Fin) : IRequest<ReporteMortalidadTransversalDto>;

public record ReporteMortalidadTransversalDto(
    int TotalBajas,
    List<MortalidadPorCausaDto> PorCausa,
    List<MortalidadDetalleDto> Detalle);

public record MortalidadPorCausaDto(string Causa, int Cantidad, decimal Porcentaje);
public record MortalidadDetalleDto(Guid Id, DateTime Fecha, string Lote, int Cantidad, string Causa);
