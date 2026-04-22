using GalponERP.Application.Auditoria.Queries.ObtenerAuditoriaLogs;
using GalponERP.Application.Auditoria.Commands.RestaurarEntidad;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GalponERP.Infrastructure.Authentication;

namespace GalponERP.Api.Controllers;

[Authorize(Policy = PolicyNames.AdminOnly)]
[ApiController]
[Route("api/[controller]")]
public class AuditoriaController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditoriaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("logs")]
    public async Task<IActionResult> ObtenerLogs(
        [FromQuery] DateTime? desde, 
        [FromQuery] DateTime? hasta, 
        [FromQuery] Guid? usuarioId, 
        [FromQuery] string? entidad)
    {
        var logs = await _mediator.Send(new ObtenerAuditoriaLogsQuery(desde, hasta, usuarioId, entidad));
        return Ok(logs);
    }

    [HttpPatch("restaurar/{entidad}/{id}")]
    public async Task<IActionResult> Restaurar(string entidad, Guid id)
    {
        await _mediator.Send(new RestaurarEntidadCommand(entidad, id));
        return NoContent();
    }
}
