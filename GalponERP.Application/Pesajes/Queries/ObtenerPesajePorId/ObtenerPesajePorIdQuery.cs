using MediatR;

namespace GalponERP.Application.Pesajes.Queries.ObtenerPesajePorId;

public record ObtenerPesajePorIdQuery(Guid Id) : IRequest<PesajeResponse?>;

public record PesajeResponse(
    Guid Id,
    Guid LoteId,
    DateTime Fecha,
    decimal PesoPromedioGramos,
    int CantidadMuestreada,
    Guid UsuarioId);
