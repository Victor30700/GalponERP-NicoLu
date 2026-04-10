using GalponERP.Application.Usuarios.Commands.ActualizarUsuario;
using GalponERP.Application.Usuarios.Commands.EliminarUsuario;
using GalponERP.Application.Usuarios.Commands.RegistrarUsuario;
using GalponERP.Application.Usuarios.Queries.ObtenerUsuarios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsuariosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { UsuarioId = id });
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var usuarios = await _mediator.Send(new ObtenerUsuariosQuery());
        return Ok(usuarios);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarUsuarioCommand command)
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
        await _mediator.Send(new EliminarUsuarioCommand(id));
        return NoContent();
    }
}
