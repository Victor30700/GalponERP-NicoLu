using GalponERP.Application.Sanidad.Commands.RegistrarBienestar;
using GalponERP.Application.Sanidad.Commands.EliminarBienestar;
using GalponERP.Application.Sanidad.Commands.ActualizarBienestar;
using GalponERP.Application.Sanidad.Queries.ObtenerHistorialBienestar;
using GalponERP.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin,Empleado")]
[ApiController]
[Route("api/[controller]")]
public class SanidadController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserContext _currentUserContext;

    public SanidadController(IMediator mediator, ICurrentUserContext currentUserContext)
    {
        _mediator = mediator;
        _currentUserContext = currentUserContext;
    }

    [HttpDelete("bienestar/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EliminarBienestar(Guid id)
    {
        if (!_currentUserContext.UsuarioId.HasValue || _currentUserContext.UsuarioId == Guid.Empty) 
            return Unauthorized("Usuario no identificado.");

        await _mediator.Send(new EliminarBienestarCommand(id, _currentUserContext.UsuarioId.Value));
        return NoContent();
    }

    [HttpPut("bienestar/{id}")]
    [Authorize(Roles = "Admin,SubAdmin")]
    public async Task<IActionResult> ActualizarBienestar(Guid id, [FromBody] ActualizarBienestarCommand command)
    {
        if (!_currentUserContext.UsuarioId.HasValue || _currentUserContext.UsuarioId == Guid.Empty) 
            return Unauthorized("Usuario no identificado.");

        if (id != command.Id) return BadRequest("El ID del comando no coincide con el ID de la URL.");

        command = command with { UsuarioId = _currentUserContext.UsuarioId.Value };
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("bienestar")]
    public async Task<IActionResult> RegistrarBienestar([FromBody] RegistrarBienestarCommand command)
    {
        try
        {
            var id = await _mediator.Send(command);
            return Ok(new { RegistroId = id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("lote/{loteId}/bienestar")]
    public async Task<IActionResult> ObtenerHistorialBienestar(Guid loteId)
    {
        var result = await _mediator.Send(new ObtenerHistorialBienestarQuery(loteId));
        return Ok(result);
    }
}
