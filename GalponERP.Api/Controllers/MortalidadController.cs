using GalponERP.Application.Mortalidad.Commands.RegistrarMortalidad;
using GalponERP.Application.Mortalidad.Queries.ObtenerMortalidadPorLote;
using GalponERP.Application.Mortalidad.Queries.ObtenerMortalidadTodas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin,Empleado")]
[ApiController]
[Route("api/[controller]")]
public class MortalidadController : ControllerBase
{
    private readonly IMediator _mediator;

    public MortalidadController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var mortalidad = await _mediator.Send(new ObtenerMortalidadTodasQuery());
        return Ok(mortalidad);
    }

    [HttpGet("lote/{loteId}")]
    public async Task<IActionResult> ObtenerPorLote(Guid loteId)
    {
        var mortalidad = await _mediator.Send(new ObtenerMortalidadPorLoteQuery(loteId));
        return Ok(mortalidad);
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarMortalidad([FromBody] RegistrarMortalidadCommand command)
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
