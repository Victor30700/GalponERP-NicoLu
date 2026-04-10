using MediatR;

namespace GalponERP.Application.Calendario.Commands.MarcarVacunaAplicada;

public record MarcarVacunaAplicadaCommand(Guid ActividadId) : IRequest;
