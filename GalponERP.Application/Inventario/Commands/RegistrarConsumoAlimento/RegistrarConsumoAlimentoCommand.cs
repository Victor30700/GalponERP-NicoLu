using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Inventario.Commands.RegistrarConsumoAlimento;

public record RegistrarConsumoAlimentoCommand(
    Guid LoteId,
    Guid ProductoId,
    decimal Cantidad,
    string? Justificacion = null) : IRequest<Guid>, IAuditableCommand;
