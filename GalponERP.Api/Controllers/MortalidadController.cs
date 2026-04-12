using GalponERP.Application.Mortalidad.Commands.RegistrarMortalidad;
using GalponERP.Application.Mortalidad.Commands.ActualizarMortalidad;
using GalponERP.Application.Mortalidad.Commands.EliminarMortalidad;
using GalponERP.Application.Mortalidad.Queries.ObtenerMortalidadPorLote;
using GalponERP.Application.Mortalidad.Queries.ObtenerMortalidadTodas;
using GalponERP.Application.Mortalidad.Queries.ObtenerReporteMortalidadTransversal;
using GalponERP.Application.Mortalidad.Queries.ObtenerMortalidadPorId;
using GalponERP.Application.Interfaces;
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
    private readonly ICurrentUserContext _currentUserContext;

    public MortalidadController(IMediator mediator, ICurrentUserContext currentUserContext)
    {
        _mediator = mediator;
        _currentUserContext = currentUserContext;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var mortalidad = await _mediator.Send(new ObtenerMortalidadTodasQuery());
        return Ok(mortalidad);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var result = await _mediator.Send(new ObtenerMortalidadPorIdQuery(id));
        return result != null ? Ok(result) : NotFound();
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
        if (!_currentUserContext.UsuarioId.HasValue || _currentUserContext.UsuarioId == Guid.Empty) 
            return Unauthorized("Usuario no identificado.");

        try
        {
            command.UsuarioId = _currentUserContext.UsuarioId.Value;
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
    public async Task<IActionResult> EliminarMortalidad(Guid id)
    {
        if (!_currentUserContext.UsuarioId.HasValue || _currentUserContext.UsuarioId == Guid.Empty) 
            return Unauthorized("Usuario no identificado.");

        try
        {
            await _mediator.Send(new EliminarMortalidadCommand(id) { UsuarioId = _currentUserContext.UsuarioId.Value });
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
