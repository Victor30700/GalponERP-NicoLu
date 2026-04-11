using GalponERP.Application.Pesajes.Commands.RegistrarPesaje;
using GalponERP.Application.Pesajes.Commands.ActualizarPesaje;
using GalponERP.Application.Pesajes.Commands.EliminarPesaje;
using GalponERP.Application.Pesajes.Queries.ObtenerPesajesPorLote;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin,Empleado")]
[ApiController]
[Route("api/[controller]")]
public class PesajesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserContext _currentUserContext;

    public PesajesController(IMediator mediator, IUsuarioRepository usuarioRepository, ICurrentUserContext currentUserContext)
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

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistrarPesajeCommand command)
    {
        var usuarioId = await GetUsuarioIdActual();
        if (usuarioId == Guid.Empty) return Unauthorized("Usuario no registrado en la base de datos.");

        command.UsuarioId = usuarioId;
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpGet("lote/{loteId}")]
    public async Task<IActionResult> ObtenerPorLote(Guid loteId)
    {
        var result = await _mediator.Send(new ObtenerPesajesPorLoteQuery(loteId));
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SubAdmin")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarPesajeCommand command)
    {
        if (id != command.Id) return BadRequest("El ID del comando no coincide con el ID de la URL.");

        var usuarioId = await GetUsuarioIdActual();
        if (usuarioId == Guid.Empty) return Unauthorized("Usuario no registrado en la base de datos.");

        try
        {
            command.UsuarioId = usuarioId;
            await _mediator.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SubAdmin")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        var usuarioId = await GetUsuarioIdActual();
        if (usuarioId == Guid.Empty) return Unauthorized("Usuario no registrado en la base de datos.");

        try
        {
            await _mediator.Send(new EliminarPesajeCommand(id) { UsuarioId = usuarioId });
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
