using GalponERP.Application.Galpones.Commands.CrearGalpon;
using GalponERP.Application.Galpones.Commands.EditarGalpon;
using GalponERP.Application.Galpones.Commands.EliminarGalpon;
using GalponERP.Application.Galpones.Queries.ListarGalpones;
using GalponERP.Application.Galpones.Queries.ObtenerGalponPorId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin,Empleado")]
[ApiController]
[Route("api/[controller]")]
public class GalponesController : ControllerBase
{
    private readonly IMediator _mediator;

    public GalponesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearGalponCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, new { GalponId = id });
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var galpones = await _mediator.Send(new ListarGalponesQuery());
        return Ok(galpones);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var result = await _mediator.Send(new ObtenerGalponPorIdQuery(id));
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(Guid id, [FromBody] EditarGalponCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("El ID de la ruta no coincide con el ID del comando.");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        await _mediator.Send(new EliminarGalponCommand(id));
        return NoContent();
    }
}
