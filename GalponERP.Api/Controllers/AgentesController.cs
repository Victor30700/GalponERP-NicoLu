using GalponERP.Application.Agentes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AgentesController : ControllerBase
{
    private readonly IAgenteOrquestadorService _agenteOrquestador;

    public AgentesController(IAgenteOrquestadorService agenteOrquestador)
    {
        _agenteOrquestador = agenteOrquestador;
    }

    [HttpPost("chat")]
    public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
    {
        try
        {
            var response = await _agenteOrquestador.ProcesarMensajeAsync(request.Mensaje);
            return Ok(new ChatResponse { Respuesta = response });
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
}

public class ChatResponse
{
    public string Respuesta { get; set; } = string.Empty;
}
