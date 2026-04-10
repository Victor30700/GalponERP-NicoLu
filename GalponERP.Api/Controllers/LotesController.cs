using GalponERP.Application.Lotes.Commands.CerrarLote;
using GalponERP.Application.Lotes.Commands.CrearLote;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public LotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearLoteCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(Crear), new { id }, new { LoteId = id });
    }

    [HttpPost("{id}/cerrar")]
    public async Task<IActionResult> Cerrar(Guid id, [FromBody] CerrarLoteCommand command)
    {
        if (id != command.LoteId)
        {
            return BadRequest("El ID del lote no coincide con el comando.");
        }

        await _mediator.Send(command);
        return NoContent();
    }
}
