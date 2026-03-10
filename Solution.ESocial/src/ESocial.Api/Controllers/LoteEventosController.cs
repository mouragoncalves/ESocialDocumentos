using ESocial.Application.DTOs;
using ESocial.Application.UseCases.ConsultarLote;
using ESocial.Application.UseCases.EnviarLote;
using ESocial.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ESocial.Api.Controllers;

[ApiController]
[Route("api/lotes")]
public class LoteEventosController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoteEventosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Envia um lote de eventos ao eSocial.
    /// POST /api/lotes
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EnviarLoteResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> EnviarLote([FromBody] LoteDto lote, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new EnviarLoteCommand(lote), cancellationToken);

        if (!result.Sucesso)
            return UnprocessableEntity(result);

        return Ok(result);
    }

    /// <summary>
    /// Consulta o resultado de processamento de um lote pelo protocolo.
    /// GET /api/lotes/{protocolo}?ambiente=2
    /// </summary>
    [HttpGet("{protocolo}")]
    [ProducesResponseType(typeof(ConsultarLoteResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarLote(
        string protocolo,
        [FromQuery] AmbienteEnvio ambiente = AmbienteEnvio.Homologacao,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ConsultarLoteQuery(protocolo, ambiente), cancellationToken);
        return Ok(result);
    }
}
