using GalponERP.Application.Lotes.Commands.CrearLote;
using GalponERP.Application.Planificacion.Queries.GetSimulacionRentabilidad;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanificacionController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlanificacionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("simulacion")]
    public async Task<IActionResult> GetSimulacion([FromQuery] GetSimulacionRentabilidadQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("lote")]
    public async Task<IActionResult> CrearLote([FromBody] CrearLoteCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { LoteId = result });
    }
}
