using GalponERP.Application.Lotes.Commands.CerrarLote;
using GalponERP.Application.Lotes.Commands.CrearLote;
using GalponERP.Application.Lotes.Commands.ActualizarLote;
using GalponERP.Application.Lotes.Commands.EliminarLote;
using GalponERP.Application.Lotes.Commands.ReabrirLote;
using GalponERP.Application.Lotes.Commands.CancelarLote;
using GalponERP.Application.Lotes.Commands.TrasladarLote;
using GalponERP.Application.Lotes.Queries.ListarLotes;
using GalponERP.Application.Lotes.Queries.ObtenerDetalleLote;
using GalponERP.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin,Empleado")]
[ApiController]
[Route("api/[controller]")]
public class LotesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserContext _currentUserContext;

    public LotesController(IMediator mediator, ICurrentUserContext currentUserContext)
    {
        _mediator = mediator;
        _currentUserContext = currentUserContext;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] bool soloActivos = true)
    {
        var lotes = await _mediator.Send(new ListarLotesQuery(soloActivos));
        return Ok(lotes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var detalle = await _mediator.Send(new ObtenerDetalleLoteQuery(id));
        if (detalle == null) return NotFound();
        return Ok(detalle);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearLoteCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, new { LoteId = id });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SubAdmin")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarLoteCommand command)
    {
        if (id != command.Id) return BadRequest("El ID del comando no coincide con el ID de la URL.");

        if (!_currentUserContext.UsuarioId.HasValue) return Unauthorized("Usuario no identificado.");

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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        if (!_currentUserContext.UsuarioId.HasValue) return Unauthorized("Usuario no identificado.");

        try
        {
            await _mediator.Send(new EliminarLoteCommand(id) { UsuarioId = _currentUserContext.UsuarioId.Value });
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/cerrar")]
    public async Task<IActionResult> Cerrar(Guid id, [FromBody]  CerrarLoteCommand command)
    {
        if (id != command.LoteId)
        {
            return BadRequest("El ID del lote no coincide con el comando.");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/reabrir")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reabrir(Guid id)
    {
        try
        {
            await _mediator.Send(new ReabrirLoteCommand(id));
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/cancelar")]
    [Authorize(Roles = "Admin,SubAdmin")]
    public async Task<IActionResult> Cancelar(Guid id, [FromBody] string justificacion)
    {
        try
        {
            await _mediator.Send(new CancelarLoteCommand(id, justificacion));
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/trasladar")]
    [Authorize(Roles = "Admin,SubAdmin")]
    public async Task<IActionResult> Trasladar(Guid id, [FromBody] Guid nuevoGalponId)
    {
        try
        {
            await _mediator.Send(new TrasladarLoteCommand(id, nuevoGalponId));
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
