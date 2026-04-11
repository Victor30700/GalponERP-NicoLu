using GalponERP.Application.Inventario.Commands.RegistrarMovimiento;
using GalponERP.Application.Inventario.Queries.ObtenerStockActual;
using GalponERP.Application.Inventario.Queries.ObtenerMovimientos;
using GalponERP.Application.Inventario.Queries.VerificarNivelesAlimento;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin,Empleado")]
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
    public async Task<IActionResult> ObtenerStock([FromQuery] Guid? productoId = null)
    {
        var stock = await _mediator.Send(new ObtenerStockActualQuery(productoId));
        return Ok(stock);
    }

    [HttpGet("productos/{id}/stock")]
    public async Task<IActionResult> ObtenerStockPorProducto(Guid id)
    {
        var stock = await _mediator.Send(new ObtenerStockActualQuery(id));
        var item = stock.FirstOrDefault();
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpGet("movimientos")]
    public async Task<IActionResult> ObtenerMovimientos()
    {
        var movimientos = await _mediator.Send(new ObtenerMovimientosQuery());
        return Ok(movimientos);
    }

    [HttpPost("movimiento")]
    public async Task<IActionResult> RegistrarMovimiento([FromBody] RegistrarMovimientoInventarioCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { MovimientoId = id });
    }

    [HttpPut("ajuste")]
    public async Task<IActionResult> AjustarInventario([FromBody] RegistrarMovimientoInventarioCommand command)
    {
        if (string.IsNullOrEmpty(command.Justificacion))
        {
            return BadRequest("Se requiere una justificación para realizar un ajuste manual.");
        }

        // Mapear el tipo a Ajuste para que no afecte FCR
        var tipoAjuste = command.Tipo == TipoMovimiento.Entrada 
            ? TipoMovimiento.AjusteEntrada 
            : TipoMovimiento.AjusteSalida;

        var adjustmentCommand = command with { Tipo = tipoAjuste };

        var id = await _mediator.Send(adjustmentCommand);
        return Ok(new { AjusteId = id });
    }

    [HttpGet("niveles-alimento")]
    public async Task<ActionResult<AlertaInventarioDto>> VerificarNivelesAlimento()
    {
        var result = await _mediator.Send(new VerificarNivelesAlimentoQuery());
        return Ok(result);
    }
}
