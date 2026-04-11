using GalponERP.Application.Finanzas.Queries.ObtenerFlujoCajaEmpresarial;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
[ApiController]
[Route("api/[controller]")]
public class FinanzasController : ControllerBase
{
    private readonly IMediator _mediator;

    public FinanzasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("flujo-caja")]
    public async Task<IActionResult> ObtenerFlujoCaja([FromQuery] DateTime inicio, [FromQuery] DateTime fin)
    {
        if (inicio == default || fin == default)
        {
            // Por defecto últimos 30 días si no se proveen fechas
            fin = DateTime.UtcNow;
            inicio = fin.AddDays(-30);
        }

        var result = await _mediator.Send(new ObtenerFlujoCajaEmpresarialQuery(inicio, fin));
        return Ok(result);
    }
}
