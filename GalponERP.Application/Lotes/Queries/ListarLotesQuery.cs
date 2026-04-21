using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Lotes.Queries.ListarLotes;

public record ListarLotesQuery(
    bool SoloActivos = true,
    string? Busqueda = null,
    int? Mes = null,
    int? Anio = null) : IRequest<IEnumerable<LoteResponse>>;

public record LoteResponse(
    Guid Id,
    string Nombre,
    string NombreLote, // Compatibilidad
    Guid GalponId,
    string GalponNombre,
    string NombreGalpon, // Compatibilidad
    DateTime FechaInicio, // Compatibilidad
    DateTime FechaIngreso,
    int CantidadInicial,
    int CantidadActual,
    int AvesVivas, // Compatibilidad
    int MortalidadAcumulada,
    int PollosVendidos,
    decimal CostoUnitarioPollito,
    int EdadSemanas,
    string Estado,
    decimal FcrActual,
    decimal MortalidadPorcentaje,
    string? Version);

public class ListarLotesQueryHandler : IRequestHandler<ListarLotesQuery, IEnumerable<LoteResponse>>
{
    private readonly ILoteRepository _loteRepository;

    public ListarLotesQueryHandler(ILoteRepository loteRepository)
    {
        _loteRepository = loteRepository;
    }

    public async Task<IEnumerable<LoteResponse>> Handle(ListarLotesQuery request, CancellationToken cancellationToken)
    {
        var lotes = await _loteRepository.ObtenerFiltradosAsync(request.Busqueda, request.Mes, request.Anio, request.SoloActivos);

        return lotes.Select(l => new LoteResponse(
            l.Id,
            l.Nombre,
            l.Nombre,
            l.GalponId,
            l.Galpon?.Nombre ?? "N/A",
            l.Galpon?.Nombre ?? "N/A",
            l.FechaIngreso, // FechaInicio
            l.FechaIngreso,
            l.CantidadInicial,
            l.CantidadActual,
            l.CantidadActual,
            l.MortalidadAcumulada,
            l.PollosVendidos,
            l.CostoUnitarioPollito.Monto,
            l.EdadSemanas,
            l.Estado.ToString(),
            0, // FCR placeholder
            l.CantidadInicial > 0 ? Math.Round((decimal)l.MortalidadAcumulada / l.CantidadInicial * 100, 1) : 0,
            l.Version.ToString()));
    }
}
