using GalponERP.Application.Ventas.Commands.RegistrarVentaParcial;
using GalponERP.Application.Ventas.Queries.ObtenerVentas;
using GalponERP.Application.Ventas.Queries.ObtenerVentaPorId;
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

    public VentasController(IMediator mediator)
    {
        _mediator = mediator;
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

    [HttpPost("parcial")]
    public async Task<IActionResult> RegistrarVentaParcial([FromBody] RegistrarVentaParcialCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, new { VentaId = id });
    }
}
