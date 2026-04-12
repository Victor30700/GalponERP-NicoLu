using GalponERP.Application.Calendario.Commands.MarcarVacunaAplicada;
using GalponERP.Application.Calendario.Queries.ObtenerCalendarioPorLote;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin,Empleado")]
[ApiController]
[Route("api/calendario")]
public class CalendarioSanitarioController : ControllerBase
{
    private readonly IMediator _mediator;

    public CalendarioSanitarioController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public record MarcarVacunaRequest(decimal CantidadConsumida);

    [HttpGet("{loteId}")]
    public async Task<IActionResult> ObtenerCalendario(Guid loteId)
    {
        var result = await _mediator.Send(new ObtenerCalendarioPorLoteQuery(loteId));
        return Ok(result);
    }

    [HttpPatch("{id}/aplicar")]
    public async Task<IActionResult> MarcarComoAplicado(Guid id, [FromBody] MarcarVacunaRequest request)
    {
        try
        {
            await _mediator.Send(new MarcarVacunaAplicadaCommand(id, request.CantidadConsumida));
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
