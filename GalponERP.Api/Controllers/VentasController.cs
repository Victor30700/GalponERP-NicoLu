using GalponERP.Application.Ventas.Commands.RegistrarVentaParcial;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VentasController : ControllerBase
{
    private readonly IMediator _mediator;

    public VentasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("parcial")]
    public async Task<IActionResult> RegistrarVentaParcial([FromBody] RegistrarVentaParcialCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(RegistrarVentaParcial), new { id }, new { VentaId = id });
    }
}
