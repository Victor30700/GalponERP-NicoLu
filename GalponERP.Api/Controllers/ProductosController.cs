using GalponERP.Application.Productos.Commands.ActualizarProducto;
using GalponERP.Application.Productos.Commands.CrearProducto;
using GalponERP.Application.Productos.Commands.EliminarProducto;
using GalponERP.Application.Productos.Queries.ListarProductos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var productos = await _mediator.Send(new ListarProductosQuery());
        return Ok(productos);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearProductoCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(Listar), new { id }, new { ProductoId = id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarProductoCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("El ID de la ruta no coincide con el ID del comando.");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        await _mediator.Send(new EliminarProductoCommand(id));
        return NoContent();
    }
}
