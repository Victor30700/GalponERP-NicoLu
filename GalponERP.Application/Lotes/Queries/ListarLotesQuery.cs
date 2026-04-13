using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Lotes.Queries.ListarLotes;

public record ListarLotesQuery(bool SoloActivos = true) : IRequest<IEnumerable<LoteResponse>>;

public record LoteResponse(
    Guid Id,
    Guid GalponId,
    string NombreGalpon,
    DateTime FechaIngreso,
    int CantidadInicial,
    int CantidadActual,
    int MortalidadAcumulada,
    int PollosVendidos,
    decimal CostoUnitarioPollito,
    int EdadSemanas,
    string Estado);

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
            l.GalponId,
            l.Galpon?.Nombre ?? "N/A",
            l.FechaIngreso,
            l.CantidadInicial,
            l.CantidadActual,
            l.MortalidadAcumulada,
            l.PollosVendidos,
            l.CostoUnitarioPollito.Monto,
            l.EdadSemanas,
            l.Estado.ToString()));
    }
}
