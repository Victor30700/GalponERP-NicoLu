using GalponERP.Application.Inventario.Queries.VerificarNivelesAlimento;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InventarioController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventarioController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("niveles-alimento")]
    public async Task<ActionResult<AlertaInventarioDto>> VerificarNivelesAlimento()
    {
        var result = await _mediator.Send(new VerificarNivelesAlimentoQuery());
        return Ok(result);
    }
}
