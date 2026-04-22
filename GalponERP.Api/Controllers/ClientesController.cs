using GalponERP.Application.Clientes.Commands.ActualizarCliente;
using GalponERP.Application.Clientes.Commands.CrearCliente;
using GalponERP.Application.Clientes.Commands.EliminarCliente;
using GalponERP.Application.Clientes.Queries.ListarClientes;
using GalponERP.Application.Clientes.Queries.ObtenerClientePorId;
using GalponERP.Application.Clientes.Queries.ObtenerHistorialCliente;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GalponERP.Infrastructure.Authentication;

namespace GalponERP.Api.Controllers;

[Authorize(Policy = PolicyNames.AnyUser)]
[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var clientes = await _mediator.Send(new ListarClientesQuery());
        return Ok(clientes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var cliente = await _mediator.Send(new ObtenerClientePorIdQuery(id));
        if (cliente == null)
        {
            return NotFound();
        }
        return Ok(cliente);
    }

    [HttpGet("{id}/historial")]
    public async Task<IActionResult> ObtenerHistorial(Guid id)
    {
        var historial = await _mediator.Send(new ObtenerHistorialClienteQuery(id));
        return Ok(historial);
    }

    [Authorize(Policy = PolicyNames.Management)]
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearClienteCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, new { ClienteId = id });
    }

    [Authorize(Policy = PolicyNames.Management)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarClienteCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("El ID de la ruta no coincide con el ID del comando.");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    [Authorize(Policy = PolicyNames.AdminOnly)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        await _mediator.Send(new EliminarClienteCommand(id));
        return NoContent();
    }
}
