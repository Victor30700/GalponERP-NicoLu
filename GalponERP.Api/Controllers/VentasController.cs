using GalponERP.Application.Ventas.Commands.RegistrarVentaParcial;
using GalponERP.Application.Ventas.Commands.AnularVenta;
using GalponERP.Application.Ventas.Queries.ObtenerVentas;
using GalponERP.Application.Ventas.Queries.ObtenerVentaPorId;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
[ApiController]
[Route("api/[controller]")]
public class VentasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserContext _currentUserContext;

    public VentasController(IMediator mediator, IUsuarioRepository usuarioRepository, ICurrentUserContext currentUserContext)
    {
        _mediator = mediator;
        _usuarioRepository = usuarioRepository;
        _currentUserContext = currentUserContext;
    }

    private async Task<Guid> GetUsuarioIdActual()
    {
        var firebaseUid = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
        if (string.IsNullOrEmpty(firebaseUid)) return Guid.Empty;

        var usuario = await _usuarioRepository.ObtenerPorFirebaseUidAsync(firebaseUid);
        
        if (usuario != null && _currentUserContext is GalponERP.Infrastructure.Authentication.CurrentUserContext context)
        {
            context.SetUser(usuario.Id, firebaseUid);
        }

        return usuario?.Id ?? Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var ventas = await _mediator.Send(new ObtenerVentasQuery());
        return Ok(ventas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var venta = await _mediator.Send(new ObtenerVentaPorIdQuery(id));
        if (venta == null) return NotFound();
        return Ok(venta);
    }

    [HttpPost("parcial")]
    public async Task<IActionResult> RegistrarVentaParcial([FromBody] RegistrarVentaParcialCommand command)
    {
        var usuarioId = await GetUsuarioIdActual();
        if (usuarioId == Guid.Empty) return Unauthorized("Usuario no registrado en la base de datos.");

        command.UsuarioId = usuarioId;
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, new { VentaId = id });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/anular")]
    public async Task<IActionResult> Anular(Guid id)
    {
        var usuarioId = await GetUsuarioIdActual();
        if (usuarioId == Guid.Empty) return Unauthorized("Usuario no registrado en la base de datos.");

        await _mediator.Send(new AnularVentaCommand(id, usuarioId));
        return NoContent();
    }
}
