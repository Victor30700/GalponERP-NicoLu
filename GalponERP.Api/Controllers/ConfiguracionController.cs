using GalponERP.Application.Configuracion.Commands.ActualizarConfiguracion;
using GalponERP.Application.Configuracion.Queries.ObtenerConfiguracion;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin,Empleado")]
[ApiController]
[Route("api/[controller]")]
public class ConfiguracionController : ControllerBase
{
    private readonly IMediator _mediator;

    public ConfiguracionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Obtener()
    {
        var config = await _mediator.Send(new ObtenerConfiguracionQuery());
        return Ok(config);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Actualizar([FromBody] ActualizarConfiguracionCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }
}
