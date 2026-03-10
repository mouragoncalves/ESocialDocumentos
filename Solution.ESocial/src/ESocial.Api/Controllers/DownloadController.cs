using ESocial.Application.DTOs;
using ESocial.Application.UseCases.SolicitarDownload;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ESocial.Api.Controllers;

[ApiController]
[Route("api/download")]
public class DownloadController : ControllerBase
{
    private readonly IMediator _mediator;

    public DownloadController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Solicita download de eventos do eSocial.
    /// POST /api/download
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SolicitarDownloadResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SolicitarDownload(
        [FromBody] SolicitacaoDownloadDto solicitacao,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SolicitarDownloadCommand(solicitacao), cancellationToken);

        if (!result.Sucesso)
            return BadRequest(result);

        return Ok(result);
    }
}
