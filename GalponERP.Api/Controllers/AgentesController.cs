using GalponERP.Application.Agentes;
using GalponERP.Application.Agentes.Chat.Commands.EliminarConversacion;
using GalponERP.Application.Agentes.Chat.Queries.ObtenerConversacionesUsuario;
using GalponERP.Application.Agentes.Chat.Queries.ObtenerHistorialChat;
using GalponERP.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AgentesController : ControllerBase
{
    private readonly IAgenteOrquestadorService _agenteOrquestador;
    private readonly IMediator _mediator;
    private readonly ICurrentUserContext _userContext;

    public AgentesController(IAgenteOrquestadorService agenteOrquestador, IMediator mediator, ICurrentUserContext userContext)
    {
        _agenteOrquestador = agenteOrquestador;
        _mediator = mediator;
        _userContext = userContext;
    }

    [HttpPost("chat")]
    public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
    {
        try
        {
            var response = await _agenteOrquestador.ProcesarMensajeAsync(request.Mensaje, request.ConversacionId);
            return Ok(new ChatResponse 
            { 
                Respuesta = response.Respuesta,
                ConversacionId = response.ConversacionId
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("conversaciones")]
    public async Task<ActionResult<IEnumerable<ConversacionResumenResponse>>> ListarConversaciones()
    {
        var usuarioId = _userContext.UsuarioId;
        if (usuarioId == null) return Unauthorized();

        var result = await _mediator.Send(new ObtenerConversacionesUsuarioQuery(usuarioId.Value));
        return Ok(result);
    }

    [HttpGet("conversaciones/{id}")]
    public async Task<ActionResult<HistorialChatResponse>> ObtenerHistorial(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new ObtenerHistorialChatQuery(id));
            if (!result.Existe)
            {
                return NotFound(new { message = $"La conversación con ID {id} no existe." });
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("conversaciones/{id}")]
    public async Task<IActionResult> EliminarConversacion(Guid id)
    {
        try
        {
            await _mediator.Send(new EliminarConversacionCommand(id));
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class ChatRequest
{
    public string Mensaje { get; set; } = string.Empty;
    public Guid? ConversacionId { get; set; }
}

public class ChatResponse
{
    public string Respuesta { get; set; } = string.Empty;
    public Guid ConversacionId { get; set; }
}
