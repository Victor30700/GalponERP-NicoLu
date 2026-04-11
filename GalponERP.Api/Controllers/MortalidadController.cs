using GalponERP.Application.Mortalidad.Commands.RegistrarMortalidad;
using GalponERP.Application.Mortalidad.Commands.ActualizarMortalidad;
using GalponERP.Application.Mortalidad.Commands.EliminarMortalidad;
using GalponERP.Application.Mortalidad.Queries.ObtenerMortalidadPorLote;
using GalponERP.Application.Mortalidad.Queries.ObtenerMortalidadTodas;
using GalponERP.Application.Mortalidad.Queries.ObtenerReporteMortalidadTransversal;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.Interfaces;
using GalponERP.Infrastructure.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin,Empleado")]
[ApiController]
[Route("api/[controller]")]
public class MortalidadController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserContext _currentUserContext;

    public MortalidadController(IMediator mediator, IUsuarioRepository usuarioRepository, ICurrentUserContext currentUserContext)
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
        
        if (usuario != null && _currentUserContext is CurrentUserContext context)
        {
            context.SetUser(usuario.Id, firebaseUid);
        }

        return usuario?.Id ?? Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var mortalidad = await _mediator.Send(new ObtenerMortalidadTodasQuery());
        return Ok(mortalidad);
    }

    [HttpGet("lote/{loteId}")]
    public async Task<IActionResult> ObtenerPorLote(Guid loteId)
    {
        var mortalidad = await _mediator.Send(new ObtenerMortalidadPorLoteQuery(loteId));
        return Ok(mortalidad);
    }

    [HttpGet("reporte-transversal")]
    [Authorize(Roles = "Admin,SubAdmin")]
    public async Task<IActionResult> ObtenerReporteTransversal([FromQuery] DateTime inicio, [FromQuery] DateTime fin)
    {
        if (inicio == default || fin == default)
        {
            fin = DateTime.UtcNow;
            inicio = fin.AddDays(-30);
        }

        var result = await _mediator.Send(new ObtenerReporteMortalidadTransversalQuery(inicio, fin));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarMortalidad([FromBody] RegistrarMortalidadCommand command)
    {
        var usuarioId = await GetUsuarioIdActual();
        if (usuarioId == Guid.Empty) return Unauthorized("Usuario no registrado en la base de datos.");

        try
        {
            command.UsuarioId = usuarioId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SubAdmin")]
    public async Task<IActionResult> ActualizarMortalidad(Guid id, [FromBody] ActualizarMortalidadCommand command)
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
    public async Task<IActionResult> EliminarMortalidad(Guid id)
    {
        var usuarioId = await GetUsuarioIdActual();
        if (usuarioId == Guid.Empty) return Unauthorized("Usuario no registrado en la base de datos.");

        try
        {
            await _mediator.Send(new EliminarMortalidadCommand(id) { UsuarioId = usuarioId });
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
