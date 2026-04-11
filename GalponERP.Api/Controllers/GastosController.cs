using GalponERP.Application.Gastos.Commands.RegistrarGastoOperativo;
using GalponERP.Application.Gastos.Queries.ObtenerGastos;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
[ApiController]
[Route("api/[controller]")]
public class GastosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserContext _currentUserContext;

    public GastosController(IMediator mediator, IUsuarioRepository usuarioRepository, ICurrentUserContext currentUserContext)
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
    public async Task<IActionResult> ObtenerGastos([FromQuery] Guid? galponId, [FromQuery] Guid? loteId)
    {
        var result = await _mediator.Send(new ObtenerGastosQuery(galponId, loteId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarGasto([FromBody] RegistrarGastoOperativoCommand command)
    {
        var usuarioId = await GetUsuarioIdActual();
        if (usuarioId == Guid.Empty) return Unauthorized("Usuario no registrado en la base de datos.");

        try
        {
            command.UsuarioId = usuarioId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
