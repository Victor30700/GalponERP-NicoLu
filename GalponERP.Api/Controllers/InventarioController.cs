using GalponERP.Application.Inventario.Commands.RegistrarMovimiento;
using GalponERP.Application.Inventario.Commands.RegistrarConsumoAlimento;
using GalponERP.Application.Inventario.Commands.RegistrarIngresoMercaderia;
using GalponERP.Application.Inventario.Commands.RegistrarPagoCompra;
using GalponERP.Application.Inventario.Commands.RegistrarConciliacion;
using GalponERP.Application.Inventario.Queries.ObtenerStockActual;
using GalponERP.Application.Inventario.Queries.ObtenerMovimientos;
using GalponERP.Application.Inventario.Queries.ObtenerReporteMovimientos;
using GalponERP.Application.Inventario.Queries.ObtenerReporteAjustes;
using GalponERP.Application.Inventario.Queries.VerificarNivelesAlimento;
using GalponERP.Application.Inventario.Queries.ObtenerKardexProducto;
using GalponERP.Application.Inventario.Queries.ObtenerValoracionInventario;
using GalponERP.Application.Inventario.Queries.ObtenerProyeccionStock;
using GalponERP.Application.Inventario.Queries.ListarPagosCompra;
using GalponERP.Application.Inventario.Commands.AnularPagoCompra;
using GalponERP.Application.Inventario.Queries.ObtenerComprasInventario;
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

    [HttpGet("valoracion")]
    public async Task<IActionResult> ObtenerValoracion()
    {
        var valoracion = await _mediator.Send(new ObtenerValoracionInventarioQuery());
        return Ok(valoracion);
    }

    [HttpGet("proyecciones")]
    public async Task<IActionResult> ObtenerProyecciones()
    {
        var proyecciones = await _mediator.Send(new ObtenerProyeccionStockQuery());
        return Ok(proyecciones);
    }

    [HttpGet("productos/{id}/stock")]
    public async Task<IActionResult> ObtenerStockPorProducto(Guid id)
    {
        var stock = await _mediator.Send(new ObtenerStockActualQuery(id));
        var item = stock.FirstOrDefault();
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpGet("productos/{id}/kardex")]
    public async Task<IActionResult> ObtenerKardex(Guid id)
    {
        var kardex = await _mediator.Send(new ObtenerKardexProductoQuery(id));
        return Ok(kardex);
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

    [HttpGet("ajustes")]
    public async Task<IActionResult> ObtenerReporteAjustes()
    {
        var reporte = await _mediator.Send(new ObtenerReporteAjustesInventarioQuery());
        return Ok(reporte);
    }

    [HttpGet("compras")]
    public async Task<IActionResult> ObtenerCompras()
    {
        var compras = await _mediator.Send(new ObtenerComprasInventarioQuery());
        return Ok(compras);
    }

    [Authorize(Roles = "Admin,SubAdmin")]
    [HttpPost("compras")]
    public async Task<IActionResult> RegistrarCompra([FromBody] RegistrarIngresoMercaderiaCommand command)
    {
        if (!_currentUserContext.UsuarioId.HasValue) 
            return Unauthorized("Usuario no identificado.");

        command.UsuarioId = _currentUserContext.UsuarioId.Value;
        var id = await _mediator.Send(command);
        return Ok(new { CompraId = id });
    }

    [Authorize(Roles = "Admin,SubAdmin")]
    [HttpPost("compras/{id}/pagos")]
    public async Task<IActionResult> RegistrarPagoCompra(Guid id, [FromBody] RegistrarPagoCompraCommand command)
    {
        if (!_currentUserContext.UsuarioId.HasValue) 
            return Unauthorized("Usuario no identificado.");

        command.CompraId = id;
        command.UsuarioId = _currentUserContext.UsuarioId.Value;
        var pagoId = await _mediator.Send(command);
        return Ok(new { PagoId = pagoId });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("compras/{compraId}/pagos/{pagoId}")]
    public async Task<IActionResult> AnularPagoCompra(Guid compraId, Guid pagoId)
    {
        if (!_currentUserContext.UsuarioId.HasValue) 
            return Unauthorized("Usuario no identificado.");

        await _mediator.Send(new AnularPagoCompraCommand(compraId, pagoId, _currentUserContext.UsuarioId.Value));
        return NoContent();
    }

    [HttpGet("compras/{id}/pagos")]
    public async Task<IActionResult> ListarPagosCompra(Guid id)
    {
        var pagos = await _mediator.Send(new ListarPagosCompraQuery(id));
        return Ok(pagos);
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

    [Authorize(Roles = "Admin,SubAdmin")]
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

    [Authorize(Roles = "Admin,SubAdmin")]
    [HttpPost("conciliacion")]
    public async Task<IActionResult> ConciliarInventario([FromBody] RegistrarConciliacionStockCommand command)
    {
        if (!_currentUserContext.UsuarioId.HasValue || _currentUserContext.UsuarioId == Guid.Empty) 
            return Unauthorized("Usuario no identificado.");

        command.UsuarioId = _currentUserContext.UsuarioId.Value;
        await _mediator.Send(command);
        return NoContent();
    }
}
