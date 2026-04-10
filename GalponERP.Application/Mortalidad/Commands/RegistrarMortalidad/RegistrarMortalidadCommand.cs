using MediatR;

namespace GalponERP.Application.Mortalidad.Commands.RegistrarMortalidad;

public record RegistrarMortalidadCommand(
    Guid LoteId,
    int Cantidad,
    string Causa,
    DateTime Fecha) : IRequest<Guid>;
