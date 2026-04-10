using GalponERP.Application.Mortalidad.Commands.RegistrarMortalidad;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MortalidadController : ControllerBase
{
    private readonly IMediator _mediator;

    public MortalidadController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarMortalidad([FromBody] RegistrarMortalidadCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
