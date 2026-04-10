using GalponERP.Application.Calendario.Commands.MarcarVacunaAplicada;
using GalponERP.Application.Calendario.Queries.ObtenerCalendarioPorLote;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CalendarioSanitarioController : ControllerBase
{
    private readonly IMediator _mediator;

    public CalendarioSanitarioController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{loteId}")]
    public async Task<IActionResult> ObtenerCalendario(Guid loteId)
    {
        var result = await _mediator.Send(new ObtenerCalendarioPorLoteQuery(loteId));
        return Ok(result);
    }

    [HttpPut("{actividadId}/aplicar")]
    public async Task<IActionResult> MarcarComoAplicado(Guid actividadId)
    {
        try
        {
            await _mediator.Send(new MarcarVacunaAplicadaCommand(actividadId));
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
