using GalponERP.Application.Pesajes.Commands.RegistrarPesaje;
using GalponERP.Application.Pesajes.Queries.ObtenerPesajesPorLote;
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

    public PesajesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistrarPesajeCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpGet("lote/{loteId}")]
    public async Task<IActionResult> ObtenerPorLote(Guid loteId)
    {
        var result = await _mediator.Send(new ObtenerPesajesPorLoteQuery(loteId));
        return Ok(result);
    }
}
