using GalponERP.Application.Proveedores.Commands.ActualizarProveedor;
using GalponERP.Application.Proveedores.Commands.CrearProveedor;
using GalponERP.Application.Proveedores.Commands.EliminarProveedor;
using GalponERP.Application.Proveedores.Queries.ListarProveedores;
using GalponERP.Application.Proveedores.Queries.ObtenerProveedorPorId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin,Empleado")]
[ApiController]
[Route("api/[controller]")]
public class ProveedoresController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProveedoresController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var proveedores = await _mediator.Send(new ListarProveedoresQuery());
        return Ok(proveedores);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var proveedor = await _mediator.Send(new ObtenerProveedorPorIdQuery(id));
        if (proveedor == null)
        {
            return NotFound();
        }
        return Ok(proveedor);
    }

    [Authorize(Roles = "Admin,SubAdmin")]
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearProveedorCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, new { ProveedorId = id });
    }

    [Authorize(Roles = "Admin,SubAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarProveedorCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("El ID de la ruta no coincide con el ID del comando.");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        await _mediator.Send(new EliminarProveedorCommand(id));
        return NoContent();
    }
}
