using GalponERP.Application.Dashboard.Queries;
using GalponERP.Application.Dashboard.Queries.ObtenerProyeccionSacrificio;
using GalponERP.Application.Dashboard.Queries.ObtenerComparativaLotes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("resumen")]
    public async Task<IActionResult> ObtenerResumen()
    {
        var resumen = await _mediator.Send(new ObtenerResumenDashboardQuery());
        return Ok(resumen);
    }

    [HttpGet("proyeccion-sacrificio/{loteId}")]
    public async Task<IActionResult> ObtenerProyeccionSacrificio(Guid loteId)
    {
        var proyeccion = await _mediator.Send(new ObtenerProyeccionSacrificioQuery(loteId));
        if (proyeccion == null) return NotFound("No hay datos suficientes para generar una proyección para este lote.");
        return Ok(proyeccion);
    }

    [HttpGet("comparativa-lotes")]
    public async Task<IActionResult> ObtenerComparativaLotes()
    {
        var comparativa = await _mediator.Send(new ObtenerComparativaLotesQuery());
        return Ok(comparativa);
    }
}

