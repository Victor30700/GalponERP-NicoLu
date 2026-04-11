using GalponERP.Application.Inventario.Commands.RegistrarMovimiento;
using GalponERP.Application.Inventario.Queries.ObtenerStockActual;
using GalponERP.Application.Inventario.Queries.VerificarNivelesAlimento;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InventarioController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventarioController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("stock")]
    public async Task<IActionResult> ObtenerStock()
    {
        var stock = await _mediator.Send(new ObtenerStockActualQuery());
        return Ok(stock);
    }

    [HttpPost("movimiento")]
    public async Task<IActionResult> RegistrarMovimiento([FromBody] RegistrarMovimientoInventarioCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { MovimientoId = id });
    }

    [HttpGet("niveles-alimento")]
    public async Task<ActionResult<AlertaInventarioDto>> VerificarNivelesAlimento()
    {
        var result = await _mediator.Send(new VerificarNivelesAlimentoQuery());
        return Ok(result);
    }
}
