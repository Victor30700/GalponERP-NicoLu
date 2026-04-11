using GalponERP.Application.Catalogos.Queries.ObtenerClientes;
using GalponERP.Application.Catalogos.Queries.ObtenerProductos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin,Empleado")]
[ApiController]
[Route("api/[controller]")]
public class CatalogosController : ControllerBase
{
    private readonly IMediator _mediator;

    public CatalogosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("clientes")]
    public async Task<IActionResult> ObtenerClientes()
    {
        var clientes = await _mediator.Send(new ObtenerClientesQuery());
        return Ok(clientes);
    }

    [HttpGet("productos")]
    public async Task<IActionResult> ObtenerProductos()
    {
        var productos = await _mediator.Send(new ObtenerProductosQuery());
        return Ok(productos);
    }
}
