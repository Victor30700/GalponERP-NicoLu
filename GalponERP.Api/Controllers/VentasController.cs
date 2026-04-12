using GalponERP.Application.Ventas.Commands.RegistrarVentaParcial;
using GalponERP.Application.Ventas.Commands.AnularVenta;
using GalponERP.Application.Ventas.Commands.RegistrarPago;
using GalponERP.Application.Ventas.Queries.ObtenerVentas;
using GalponERP.Application.Ventas.Queries.ObtenerVentaPorId;
using GalponERP.Application.Ventas.Queries.ObtenerVentasPorLote;
using GalponERP.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
[ApiController]
[Route("api/[controller]")]
public class VentasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserContext _currentUserContext;

    public VentasController(IMediator mediator, ICurrentUserContext currentUserContext)
    {
        _mediator = mediator;
        _currentUserContext = currentUserContext;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var ventas = await _mediator.Send(new ObtenerVentasQuery());
        return Ok(ventas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var venta = await _mediator.Send(new ObtenerVentaPorIdQuery(id));
        if (venta == null) return NotFound();
        return Ok(venta);
    }

    [HttpGet("lote/{loteId}")]
    public async Task<IActionResult> ObtenerPorLote(Guid loteId)
    {
        var ventas = await _mediator.Send(new ObtenerVentasPorLoteQuery(loteId));
        return Ok(ventas);
    }

    [HttpPost("parcial")]
    public async Task<IActionResult> RegistrarVentaParcial([FromBody] RegistrarVentaParcialCommand command)
    {
        if (!_currentUserContext.UsuarioId.HasValue || _currentUserContext.UsuarioId == Guid.Empty) 
            return Unauthorized("Usuario no identificado.");

        command.UsuarioId = _currentUserContext.UsuarioId.Value;
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, new { VentaId = id });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/anular")]
    public async Task<IActionResult> Anular(Guid id)
    {
        if (!_currentUserContext.UsuarioId.HasValue || _currentUserContext.UsuarioId == Guid.Empty) 
            return Unauthorized("Usuario no identificado.");

        await _mediator.Send(new AnularVentaCommand(id, _currentUserContext.UsuarioId.Value));
        return NoContent();
    }

    [HttpPost("{id}/pagos")]
    public async Task<IActionResult> RegistrarPago(Guid id, [FromBody] RegistrarPagoVentaCommand command)
    {
        if (!_currentUserContext.UsuarioId.HasValue || _currentUserContext.UsuarioId == Guid.Empty) 
            return Unauthorized("Usuario no identificado.");

        command.VentaId = id;
        command.UsuarioId = _currentUserContext.UsuarioId.Value;

        var pagoId = await _mediator.Send(command);
        return Ok(new { PagoId = pagoId });
    }
}
