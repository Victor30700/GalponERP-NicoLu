using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Lotes.Queries.ListarLotes;

public record ListarLotesQuery(bool SoloActivos = true) : IRequest<IEnumerable<LoteResponse>>;

public record LoteResponse(
    Guid Id,
    string NombreLote,
    Guid GalponId,
    string GalponNombre,
    string NombreGalpon,
    DateTime FechaInicio,
    DateTime FechaIngreso,
    int CantidadInicial,
    int AvesVivas,
    int CantidadActual,
    int MortalidadAcumulada,
    int PollosVendidos,
    decimal CostoUnitarioPollito,
    int EdadSemanas,
    string Estado,
    decimal FcrActual,
    decimal MortalidadPorcentaje);

public class ListarLotesQueryHandler : IRequestHandler<ListarLotesQuery, IEnumerable<LoteResponse>>
{
    private readonly ILoteRepository _loteRepository;

    public ListarLotesQueryHandler(ILoteRepository loteRepository)
    {
        _loteRepository = loteRepository;
    }

    public async Task<IEnumerable<LoteResponse>> Handle(ListarLotesQuery request, CancellationToken cancellationToken)
    {
        var lotes = request.SoloActivos
            ? await _loteRepository.ObtenerActivosAsync()
            : await _loteRepository.ObtenerTodosAsync();

        return lotes.Select(l => new LoteResponse(
            l.Id,
            $"Lote {l.FechaIngreso:ddMM}-{l.Galpon?.Nombre ?? "N/A"}",
            l.GalponId,
            l.Galpon?.Nombre ?? "N/A",
            l.Galpon?.Nombre ?? "N/A",
            l.FechaIngreso,
            l.FechaIngreso,
            l.CantidadInicial,
            l.CantidadActual,
            l.CantidadActual,
            l.MortalidadAcumulada,
            l.PollosVendidos,
            l.CostoUnitarioPollito.Monto,
            l.EdadSemanas,
            l.Estado.ToString(),
            0, // FCR placeholder for list (expensive to calculate here)
            l.CantidadInicial > 0 ? Math.Round((decimal)l.MortalidadAcumulada / l.CantidadInicial * 100, 1) : 0));
    }
}
