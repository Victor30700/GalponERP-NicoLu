using GalponERP.Application.PlantillasSanitarias.Commands.CrearPlantilla;
using GalponERP.Application.PlantillasSanitarias.Commands.ActualizarPlantilla;
using GalponERP.Application.PlantillasSanitarias.Commands.EliminarPlantillaSanitaria;
using GalponERP.Application.PlantillasSanitarias.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
[ApiController]
[Route("api/[controller]")]
public class PlantillasController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlantillasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var result = await _mediator.Send(new ObtenerPlantillasQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var result = await _mediator.Send(new ObtenerPlantillaPorIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearPlantillaCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, new { Id = id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarPlantillaCommand command)
    {
        if (id != command.Id) return BadRequest("El ID del comando no coincide con el de la URL.");
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        await _mediator.Send(new EliminarPlantillaSanitariaCommand(id));
        return NoContent();
    }
}
