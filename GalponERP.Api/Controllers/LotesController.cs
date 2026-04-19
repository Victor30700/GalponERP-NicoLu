using GalponERP.Application.Lotes.Commands.CerrarLote;
using GalponERP.Application.Lotes.Commands.CrearLote;
using GalponERP.Application.Lotes.Commands.ActualizarLote;
using GalponERP.Application.Lotes.Commands.EliminarLote;
using GalponERP.Application.Lotes.Commands.ReabrirLote;
using GalponERP.Application.Lotes.Commands.CancelarLote;
using GalponERP.Application.Lotes.Commands.TrasladarLote;
using GalponERP.Application.Nutricion.Formulas.Commands.RegistrarConsumoFormula;
using GalponERP.Application.Lotes.Queries.ListarLotes;
using GalponERP.Application.Lotes.Queries.ObtenerDetalleLote;
using GalponERP.Application.Lotes.Queries.ObtenerConsumoAlimentoPdf;
using GalponERP.Application.Lotes.Queries.ObtenerFichaSemanalPdf;
using GalponERP.Application.Lotes.Queries.ObtenerIngresoLotePdf;
using GalponERP.Application.Lotes.Queries.ObtenerMortalidadPdf;
using GalponERP.Application.Lotes.Queries.ObtenerLiquidacionLotePdf;
using GalponERP.Application.Lotes.Queries.ObtenerReporteCierrePdf;
using GalponERP.Application.Lotes.Queries.ObtenerRendimientoVivo;
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
    private readonly ICurrentUserContext _currentUserContext;

    public LotesController(IMediator mediator, ICurrentUserContext currentUserContext)
    {
        _mediator = mediator;
        _currentUserContext = currentUserContext;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] bool soloActivos = true,
        [FromQuery] string? busqueda = null,
        [FromQuery] int? mes = null,
        [FromQuery] int? anio = null)
    {
        var lotes = await _mediator.Send(new ListarLotesQuery(soloActivos, busqueda, mes, anio));
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

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SubAdmin")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarLoteCommand command)
    {
        if (id != command.Id) return BadRequest("El ID del comando no coincide con el ID de la URL.");

        if (!_currentUserContext.UsuarioId.HasValue) return Unauthorized("Usuario no identificado.");

        command.UsuarioId = _currentUserContext.UsuarioId.Value;
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        if (!_currentUserContext.UsuarioId.HasValue) return Unauthorized("Usuario no identificado.");

        await _mediator.Send(new EliminarLoteCommand(id) { UsuarioId = _currentUserContext.UsuarioId.Value });
        return NoContent();
    }

    [HttpPost("{id}/cerrar")]
    public async Task<IActionResult> Cerrar(Guid id, [FromBody]  CerrarLoteCommand command)
    {
        if (id != command.LoteId)
        {
            return BadRequest("El ID del lote no coincide con el comando.");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}/reabrir")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reabrir(Guid id)
    {
        await _mediator.Send(new ReabrirLoteCommand(id));
        return NoContent();
    }

    [HttpPost("{id}/cancelar")]
    [Authorize(Roles = "Admin,SubAdmin")]
    public async Task<IActionResult> Cancelar(Guid id, [FromBody] string justificacion)
    {
        await _mediator.Send(new CancelarLoteCommand(id, justificacion));
        return NoContent();
    }

    [HttpGet("{id}/rendimiento-vivo")]
    public async Task<IActionResult> ObtenerRendimientoVivo(Guid id)
    {
        var result = await _mediator.Send(new ObtenerRendimientoVivoLoteQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("{id}/reporte-cierre-pdf")]
    public async Task<IActionResult> ObtenerReporteCierrePdf(Guid id)
    {
        var pdfBytes = await _mediator.Send(new ObtenerReporteCierreLotePdfQuery(id));
        return File(pdfBytes, "application/pdf", $"Liquidacion_Lote_{id}.pdf");
    }

    [HttpGet("{id}/reportes/ingreso")]
    public async Task<IActionResult> ObtenerReporteIngreso(Guid id)
    {
        var pdfBytes = await _mediator.Send(new ObtenerIngresoLotePdfQuery(id));
        return File(pdfBytes, "application/pdf", $"SAVCO-01_Ingreso_Lote_{id}.pdf");
    }

    [HttpGet("{id}/reportes/mortalidad")]
    public async Task<IActionResult> ObtenerReporteMortalidad(Guid id)
    {
        var pdfBytes = await _mediator.Send(new ObtenerMortalidadPdfQuery(id));
        return File(pdfBytes, "application/pdf", $"SAVCO-02_Mortalidad_Lote_{id}.pdf");
    }

    [HttpGet("{id}/reportes/semanal")]
    public async Task<IActionResult> ObtenerReporteSemanal(Guid id)
    {
        var pdfBytes = await _mediator.Send(new ObtenerFichaSemanalPdfQuery(id));
        return File(pdfBytes, "application/pdf", $"SAVCO-03_Ficha_Semanal_Lote_{id}.pdf");
    }

    [HttpGet("{id}/reportes/consumo")]
    public async Task<IActionResult> ObtenerReporteConsumo(Guid id)
    {
        var pdfBytes = await _mediator.Send(new ObtenerConsumoAlimentoPdfQuery(id));
        return File(pdfBytes, "application/pdf", $"SAVCO-04_Consumo_Alimento_Lote_{id}.pdf");
    }

    [HttpGet("{id}/reportes/liquidacion")]
    public async Task<IActionResult> ObtenerReporteLiquidacion(Guid id)
    {
        var pdfBytes = await _mediator.Send(new ObtenerLiquidacionLotePdfQuery(id));
        return File(pdfBytes, "application/pdf", $"SAVCO-09_Liquidacion_Lote_{id}.pdf");
    }

    [HttpPost("{id}/trasladar")]
    [Authorize(Roles = "Admin,SubAdmin")]
    public async Task<IActionResult> Trasladar(Guid id, [FromBody] Guid nuevoGalponId)
    {
        await _mediator.Send(new TrasladarLoteCommand(id, nuevoGalponId));
        return NoContent();
    }

    [HttpPost("{id}/consumo-formula")]
    public async Task<IActionResult> RegistrarConsumoFormula(Guid id, [FromBody] RegistrarConsumoFormulaCommand command)
    {
        if (id != command.LoteId) return BadRequest("El ID del lote no coincide con el comando.");
        var result = await _mediator.Send(command);
        return Ok(new { MovimientoId = result });
    }
}
