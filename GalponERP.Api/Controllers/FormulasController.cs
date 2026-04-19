using GalponERP.Application.Nutricion.Formulas.Commands.CrearFormula;
using GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula;
using GalponERP.Application.Nutricion.Formulas.Commands.EliminarFormula;
using GalponERP.Application.Nutricion.Formulas.Queries.GetFormulas;
using GalponERP.Application.Nutricion.Formulas.Queries.GetFormulaById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin,Empleado")]
[ApiController]
[Route("api/[controller]")]
public class FormulasController : ControllerBase
{
    private readonly IMediator _mediator;

    public FormulasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var result = await _mediator.Send(new GetFormulasQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var result = await _mediator.Send(new GetFormulaByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SubAdmin")]
    public async Task<IActionResult> Crear([FromBody] CrearFormulaCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, new { FormulaId = id });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SubAdmin")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarFormulaCommand command)
    {
        if (id != command.Id) return BadRequest("ID de la URL no coincide con el comando.");
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SubAdmin")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        await _mediator.Send(new EliminarFormulaCommand(id));
        return NoContent();
    }
}
