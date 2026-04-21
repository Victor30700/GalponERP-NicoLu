using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Lotes.Commands.ReabrirLote;

public record ReabrirLoteCommand(Guid LoteId) : IRequest<Unit>, IAuditableCommand;
