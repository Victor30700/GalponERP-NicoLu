using GalponERP.Application.Pesajes.Commands.RegistrarPesaje;
using GalponERP.Application.Pesajes.Commands.ActualizarPesaje;
using GalponERP.Application.Pesajes.Commands.EliminarPesaje;
using GalponERP.Application.Pesajes.Queries.ObtenerPesajesPorLote;
using GalponERP.Application.Pesajes.Queries.ObtenerPesajePorId;
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
    private readonly ICurrentUserContext _currentUserContext;

    public PesajesController(IMediator mediator, ICurrentUserContext currentUserContext)
    {
        _mediator = mediator;
        _currentUserContext = currentUserContext;
    }

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistrarPesajeCommand command)
    {
        if (!_currentUserContext.UsuarioId.HasValue || _currentUserContext.UsuarioId == Guid.Empty) 
            return Unauthorized("Usuario no identificado.");

        command.UsuarioId = _currentUserContext.UsuarioId.Value;
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var result = await _mediator.Send(new ObtenerPesajePorIdQuery(id));
        return result != null ? Ok(result) : NotFound();
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
        if (!_currentUserContext.UsuarioId.HasValue || _currentUserContext.UsuarioId == Guid.Empty) 
            return Unauthorized("Usuario no identificado.");

        if (id != command.Id) return BadRequest("El ID del comando no coincide con el ID de la URL.");

        try
        {
            command.UsuarioId = _currentUserContext.UsuarioId.Value;
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
        if (!_currentUserContext.UsuarioId.HasValue || _currentUserContext.UsuarioId == Guid.Empty) 
            return Unauthorized("Usuario no identificado.");

        try
        {
            await _mediator.Send(new EliminarPesajeCommand(id) { UsuarioId = _currentUserContext.UsuarioId.Value });
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
