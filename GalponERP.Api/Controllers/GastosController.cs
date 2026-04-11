using GalponERP.Application.Gastos.Commands.RegistrarGastoOperativo;
using GalponERP.Application.Gastos.Commands.ActualizarGastoOperativo;
using GalponERP.Application.Gastos.Commands.EliminarGastoOperativo;
using GalponERP.Application.Gastos.Queries.ObtenerGastos;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
[ApiController]
[Route("api/[controller]")]
public class GastosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserContext _currentUserContext;

    public GastosController(IMediator mediator, IUsuarioRepository usuarioRepository, ICurrentUserContext currentUserContext)
    {
        _mediator = mediator;
        _usuarioRepository = usuarioRepository;
        _currentUserContext = currentUserContext;
    }

    private async Task<Guid> GetUsuarioIdActual()
    {
        var firebaseUid = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
        if (string.IsNullOrEmpty(firebaseUid)) return Guid.Empty;

        var usuario = await _usuarioRepository.ObtenerPorFirebaseUidAsync(firebaseUid);
        
        if (usuario != null && _currentUserContext is GalponERP.Infrastructure.Authentication.CurrentUserContext context)
        {
            context.SetUser(usuario.Id, firebaseUid);
        }

        return usuario?.Id ?? Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerGastos([FromQuery] Guid? galponId, [FromQuery] Guid? loteId)
    {
        var result = await _mediator.Send(new ObtenerGastosQuery(galponId, loteId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarGasto([FromBody] RegistrarGastoOperativoCommand command)
    {
        var usuarioId = await GetUsuarioIdActual();
        if (usuarioId == Guid.Empty) return Unauthorized("Usuario no registrado en la base de datos.");

        command.UsuarioId = usuarioId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarGasto(Guid id, [FromBody] ActualizarGastoOperativoCommand command)
    {
        var usuarioId = await GetUsuarioIdActual();
        if (usuarioId == Guid.Empty) return Unauthorized("Usuario no registrado en la base de datos.");

        if (id != command.Id)
        {
            return BadRequest(new { message = "El ID de la ruta no coincide con el ID del comando." });
        }

        command.UsuarioId = usuarioId;
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarGasto(Guid id)
    {
        var usuarioId = await GetUsuarioIdActual();
        if (usuarioId == Guid.Empty) return Unauthorized("Usuario no registrado en la base de datos.");

        var command = new EliminarGastoOperativoCommand(id)
        {
            UsuarioId = usuarioId
        };
        await _mediator.Send(command);
        return NoContent();
    }
}
