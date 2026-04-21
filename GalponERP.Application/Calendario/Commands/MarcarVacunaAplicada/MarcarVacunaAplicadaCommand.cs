using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Calendario.Commands.MarcarVacunaAplicada;

public record MarcarVacunaAplicadaCommand(Guid ActividadId, decimal CantidadConsumida) : IRequest, IAuditableCommand;
