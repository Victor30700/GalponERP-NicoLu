using GalponERP.Application.Finanzas.Queries.ObtenerFlujoCajaEmpresarial;
using GalponERP.Application.Finanzas.Queries.ObtenerCuentasPorCobrar;
using GalponERP.Application.Finanzas.Queries.ObtenerCuentasPorPagar;
using GalponERP.Application.Finanzas.Queries.ObtenerGastosPorCategoria;
using GalponERP.Application.Finanzas.Queries.ObtenerGastosGlobales;
using GalponERP.Application.Finanzas.Queries.ObtenerFlujoCajaProyectado;
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

    [HttpGet("gastos-por-categoria")]
    public async Task<IActionResult> ObtenerGastosPorCategoria([FromQuery] DateTime inicio, [FromQuery] DateTime fin)
    {
        if (inicio == default || fin == default)
        {
            fin = DateTime.UtcNow;
            inicio = fin.AddDays(-30);
        }

        var result = await _mediator.Send(new ObtenerGastosPorCategoriaQuery(inicio, fin));
        return Ok(result);
    }

    [HttpGet("gastos")]
    public async Task<IActionResult> ObtenerGastosGlobales([FromQuery] DateTime? inicio, [FromQuery] DateTime? fin, [FromQuery] string? categoria)
    {
        var result = await _mediator.Send(new ObtenerGastosGlobalesQuery(inicio, fin, categoria));
        return Ok(result);
    }

    [HttpGet("cuentas-por-cobrar")]
    public async Task<IActionResult> ObtenerCuentasPorCobrar()
    {
        var result = await _mediator.Send(new ObtenerCuentasPorCobrarQuery());
        return Ok(result);
    }

    [HttpGet("cuentas-por-pagar")]
    public async Task<IActionResult> ObtenerCuentasPorPagar()
    {
        var result = await _mediator.Send(new ObtenerCuentasPorPagarQuery());
        return Ok(result);
    }

    [HttpGet("flujo-proyectado")]
    public async Task<IActionResult> ObtenerFlujoProyectado()
    {
        var result = await _mediator.Send(new ObtenerFlujoCajaProyectadoQuery());
        return Ok(result);
    }
}
