using GalponERP.Application.Lotes.Commands.CerrarLote;
using GalponERP.Application.Lotes.Commands.CrearLote;
using GalponERP.Application.Lotes.Queries.ListarLotes;
using GalponERP.Application.Lotes.Queries.ObtenerDetalleLote;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize(Roles = "Admin,SubAdmin,Empleado")]
[ApiController]
[Route("api/[controller]")]
public class LotesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserContext _currentUserContext;

    public LotesController(IMediator mediator, IUsuarioRepository usuarioRepository, ICurrentUserContext currentUserContext)
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
    public async Task<IActionResult> Listar([FromQuery] bool soloActivos = true)
    {
        var lotes = await _mediator.Send(new ListarLotesQuery(soloActivos));
        return Ok(lotes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var detalle = await _mediator.Send(new ObtenerDetalleLoteQuery(id));
        if (detalle == null) return NotFound();
        return Ok(detalle);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearLoteCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, new { LoteId = id });
    }

    [HttpPost("{id}/cerrar")]
    public async Task<IActionResult> Cerrar(Guid id, [FromBody] CerrarLoteCommand command)
    {
        if (id != command.LoteId)
        {
            return BadRequest("El ID del lote no coincide con el comando.");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
