using GalponERP.Application.Usuarios.Commands.ActualizarUsuario;
using GalponERP.Application.Usuarios.Commands.EliminarUsuario;
using GalponERP.Application.Usuarios.Commands.RegistrarUsuario;
using GalponERP.Application.Usuarios.Queries.ObtenerUsuarios;
using GalponERP.Application.Usuarios.Queries.ObtenerUsuarioActual;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    [Authorize(Roles = "Admin,SubAdmin,Empleado")]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var firebaseUid = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
        if (string.IsNullOrEmpty(firebaseUid))
        {
            return Unauthorized();
        }

        var usuario = await _mediator.Send(new ObtenerUsuarioActualQuery(firebaseUid));
        if (usuario == null)
        {
            return NotFound("Usuario no registrado en la base de datos local.");
        }

        return Ok(usuario);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { UsuarioId = id });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var usuarios = await _mediator.Send(new ObtenerUsuariosQuery());
        return Ok(usuarios);
    }

    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        await _mediator.Send(new EliminarUsuarioCommand(id));
        return NoContent();
    }
}
