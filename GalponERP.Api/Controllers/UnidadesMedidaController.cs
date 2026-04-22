using GalponERP.Application.Catalogos.UnidadesMedida.Commands.ActualizarUnidadMedida;
using GalponERP.Application.Catalogos.UnidadesMedida.Commands.CrearUnidadMedida;
using GalponERP.Application.Catalogos.UnidadesMedida.Commands.EliminarUnidadMedida;
using GalponERP.Application.Catalogos.UnidadesMedida.Queries.ListarUnidadesMedida;
using GalponERP.Application.Catalogos.UnidadesMedida.Queries.ObtenerUnidadMedidaPorId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GalponERP.Infrastructure.Authentication;

namespace GalponERP.Api.Controllers;

[Authorize(Policy = PolicyNames.AnyUser)]
[ApiController]
[Route("api/[controller]")]
public class UnidadesMedidaController : ControllerBase
{
    private readonly IMediator _mediator;

    public UnidadesMedidaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        return Ok(await _mediator.Send(new ListarUnidadesMedidaQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var result = await _mediator.Send(new ObtenerUnidadMedidaPorIdQuery(id));
        return result != null ? Ok(result) : NotFound();
    }

    [Authorize(Policy = PolicyNames.Management)]
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearUnidadMedidaCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, id);
    }

    [Authorize(Policy = PolicyNames.Management)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarUnidadMedidaCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch");
        await _mediator.Send(command);
        return NoContent();
    }

    [Authorize(Policy = PolicyNames.AdminOnly)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        await _mediator.Send(new EliminarUnidadMedidaCommand(id));
        return NoContent();
    }
}
