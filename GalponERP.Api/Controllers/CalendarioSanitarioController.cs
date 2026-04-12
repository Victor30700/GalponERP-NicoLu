using GalponERP.Application.Calendario.Commands.MarcarVacunaAplicada;
using GalponERP.Application.Calendario.Commands;
using GalponERP.Application.Calendario.Queries.ObtenerCalendarioPorLote;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin,Empleado")]
[ApiController]
[Route("api/[controller]")]
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

    [Authorize(Roles = "Admin,SubAdmin")]
    [HttpPost("actividad-manual")]
    public async Task<IActionResult> AgregarActividadManual([FromBody] AgregarActividadManualCommand command)
    {
        try
        {
            var id = await _mediator.Send(command);
            return Ok(new { ActividadId = id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin,SubAdmin")]
    [HttpPut("{id}/reprogramar")]
    public async Task<IActionResult> ReprogramarActividad(Guid id, [FromBody] ReprogramarActividadCommand command)
    {
        if (id != command.ActividadId)
        {
            return BadRequest("El ID de la ruta no coincide con el del comando.");
        }

        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
