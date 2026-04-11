using GalponERP.Application.Catalogos.Categorias.Commands.ActualizarCategoria;
using GalponERP.Application.Catalogos.Categorias.Commands.CrearCategoria;
using GalponERP.Application.Catalogos.Categorias.Commands.EliminarCategoria;
using GalponERP.Application.Catalogos.Categorias.Queries.ListarCategorias;
using GalponERP.Application.Catalogos.Categorias.Queries.ObtenerCategoriaPorId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin,Empleado")]
[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        return Ok(await _mediator.Send(new ListarCategoriasQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var result = await _mediator.Send(new ObtenerCategoriaPorIdQuery(id));
        return result != null ? Ok(result) : NotFound();
    }

    [Authorize(Roles = "Admin,SubAdmin")]
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearCategoriaCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, id);
    }

    [Authorize(Roles = "Admin,SubAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarCategoriaCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch");
        await _mediator.Send(command);
        return NoContent();
    }

    [Authorize(Roles = "Admin,SubAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        await _mediator.Send(new EliminarCategoriaCommand(id));
        return NoContent();
    }
}
