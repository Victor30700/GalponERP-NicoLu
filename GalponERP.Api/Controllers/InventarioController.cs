using GalponERP.Application.Inventario.Commands.RegistrarMovimiento;
using GalponERP.Application.Inventario.Commands.RegistrarConsumoAlimento;
using GalponERP.Application.Inventario.Queries.ObtenerStockActual;
using GalponERP.Application.Inventario.Queries.ObtenerMovimientos;
using GalponERP.Application.Inventario.Queries.ObtenerReporteMovimientos;
using GalponERP.Application.Inventario.Queries.VerificarNivelesAlimento;
using GalponERP.Domain.Entities;
using GalponERP.Application.Interfaces;
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
    private readonly ICurrentUserContext _currentUserContext;

    public InventarioController(IMediator mediator, ICurrentUserContext currentUserContext)
    {
        _mediator = mediator;
        _currentUserContext = currentUserContext;
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

    [HttpGet("productos/{id}/movimientos")]
    public async Task<IActionResult> ObtenerMovimientosPorProducto(Guid id)
    {
        var movimientos = await _mediator.Send(new ObtenerMovimientosQuery(id));
        return Ok(movimientos);
    }

    [HttpGet("movimientos/reporte")]
    public async Task<IActionResult> ObtenerReporteMovimientos([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin, [FromQuery] Guid? categoriaProductoId = null)
    {
        var reporte = await _mediator.Send(new ObtenerReporteMovimientosQuery(fechaInicio, fechaFin, categoriaProductoId));
        return Ok(reporte);
    }

    [HttpPost("movimiento")]
    public async Task<IActionResult> RegistrarMovimiento([FromBody] RegistrarMovimientoInventarioCommand command)
    {
        if (!_currentUserContext.UsuarioId.HasValue || _currentUserContext.UsuarioId == Guid.Empty) 
            return Unauthorized("Usuario no identificado.");

        command.UsuarioId = _currentUserContext.UsuarioId.Value;
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

        if (!_currentUserContext.UsuarioId.HasValue || _currentUserContext.UsuarioId == Guid.Empty) 
            return Unauthorized("Usuario no identificado.");

        // Mapear el tipo a Ajuste para que no afecte FCR
        var tipoAjuste = command.Tipo == TipoMovimiento.Entrada 
            ? TipoMovimiento.AjusteEntrada 
            : TipoMovimiento.AjusteSalida;

        var finalCommand = command with { Tipo = tipoAjuste };
        finalCommand.UsuarioId = _currentUserContext.UsuarioId.Value;

        var id = await _mediator.Send(finalCommand);
        return Ok(new { AjusteId = id });
    }

    [HttpPost("consumo-diario")]
    public async Task<IActionResult> RegistrarConsumoDiario([FromBody] RegistrarConsumoAlimentoCommand command)
    {
        try
        {
            // Nota: RegistrarConsumoAlimentoCommand debería extraer el UsuarioId en su Handler o via ICurrentUserContext
            // pero para consistencia con los otros controllers lo manejaremos aquí si el comando lo expone.
            // Si el comando no tiene UsuarioId, el Handler debe usar ICurrentUserContext.
            var id = await _mediator.Send(command);
            return Ok(new { MovimientoId = id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("niveles-alimento")]
    public async Task<ActionResult<AlertaInventarioDto>> VerificarNivelesAlimento()
    {
        var result = await _mediator.Send(new VerificarNivelesAlimentoQuery());
        return Ok(result);
    }
}
