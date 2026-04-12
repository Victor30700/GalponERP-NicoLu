using GalponERP.Application.Inventario.Commands.RegistrarMovimiento;
using GalponERP.Application.Inventario.Commands.RegistrarConsumoAlimento;
using GalponERP.Application.Inventario.Queries.ObtenerStockActual;
using GalponERP.Application.Inventario.Queries.ObtenerMovimientos;
using GalponERP.Application.Inventario.Queries.ObtenerReporteMovimientos;
using GalponERP.Application.Inventario.Queries.VerificarNivelesAlimento;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
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
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserContext _currentUserContext;

    public InventarioController(IMediator mediator, IUsuarioRepository usuarioRepository, ICurrentUserContext currentUserContext)
    {
        _mediator = mediator;
        _usuarioRepository = usuarioRepository;
        _currentUserContext = currentUserContext;
    }

    private async Task<Guid> GetUsuarioIdActual()
    {
        var firebaseUid = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
        if (string.IsNullOrEmpty(firebaseUid)) return Guid.Empty;

        var usuario = await _usuarioRepository.ObtenerPorFirebaseUidAsync(firebaseUid);
        
        if (usuario != null && _currentUserContext is GalponERP.Infrastructure.Authentication.CurrentUserContext context)
        {
            context.SetUser(usuario.Id, firebaseUid);
        }

        return usuario?.Id ?? Guid.Empty;
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
        var usuarioId = await GetUsuarioIdActual();
        if (usuarioId == Guid.Empty) return Unauthorized("Usuario no registrado en la base de datos.");

        command.UsuarioId = usuarioId;
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

        var usuarioId = await GetUsuarioIdActual();
        if (usuarioId == Guid.Empty) return Unauthorized("Usuario no registrado en la base de datos.");

        // Mapear el tipo a Ajuste para que no afecte FCR
        var tipoAjuste = command.Tipo == TipoMovimiento.Entrada 
            ? TipoMovimiento.AjusteEntrada 
            : TipoMovimiento.AjusteSalida;

        // No podemos usar 'with' para UsuarioId, así que lo seteamos manual
        var finalCommand = command with { Tipo = tipoAjuste };
        finalCommand.UsuarioId = usuarioId;

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
}
