using GalponERP.Application.Gastos.Commands.RegistrarGastoOperativo;
using GalponERP.Application.Gastos.Queries.ObtenerGastos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GastosController : ControllerBase
{
    private readonly IMediator _mediator;

    public GastosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerGastos([FromQuery] Guid? galponId, [FromQuery] Guid? loteId)
    {
        var result = await _mediator.Send(new ObtenerGastosQuery(galponId, loteId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarGasto([FromBody] RegistrarGastoOperativoCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
