using GalponERP.Application.Gastos.Commands.RegistrarGastoOperativo;
using GalponERP.Application.Gastos.Commands.ActualizarGastoOperativo;
using GalponERP.Application.Gastos.Commands.EliminarGastoOperativo;
using GalponERP.Application.Gastos.Queries.ObtenerGastos;
using GalponERP.Application.Gastos.Queries.ObtenerGastoOperativoPorId;
using GalponERP.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin,Empleado")]
[ApiController]
[Route("api/[controller]")]
public class GastosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserContext _currentUserContext;

    public GastosController(IMediator mediator, ICurrentUserContext currentUserContext)
    {
        _mediator = mediator;
        _currentUserContext = currentUserContext;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerGastos([FromQuery] Guid? galponId, [FromQuery] Guid? loteId)
    {
        var result = await _mediator.Send(new ObtenerGastosQuery(galponId, loteId));
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var result = await _mediator.Send(new ObtenerGastoOperativoPorIdQuery(id));
        return result != null ? Ok(result) : NotFound();
    }

    [Authorize(Roles = "Admin,SubAdmin")]
    [HttpPost]
    public async Task<IActionResult> RegistrarGasto([FromBody] RegistrarGastoOperativoCommand command)
    {
        if (!_currentUserContext.UsuarioId.HasValue || _currentUserContext.UsuarioId == Guid.Empty) 
            return Unauthorized("Usuario no identificado.");

        command.UsuarioId = _currentUserContext.UsuarioId.Value;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,SubAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarGasto(Guid id, [FromBody] ActualizarGastoOperativoCommand command)
    {
        if (!_currentUserContext.UsuarioId.HasValue || _currentUserContext.UsuarioId == Guid.Empty) 
            return Unauthorized("Usuario no identificado.");

        if (id != command.Id)
        {
            return BadRequest(new { message = "El ID de la ruta no coincide con el ID del comando." });
        }

        command.UsuarioId = _currentUserContext.UsuarioId.Value;
        await _mediator.Send(command);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarGasto(Guid id)
    {
        if (!_currentUserContext.UsuarioId.HasValue || _currentUserContext.UsuarioId == Guid.Empty) 
            return Unauthorized("Usuario no identificado.");

        var command = new EliminarGastoOperativoCommand(id)
        {
            UsuarioId = _currentUserContext.UsuarioId.Value
        };
        await _mediator.Send(command);
        return NoContent();
    }
}
