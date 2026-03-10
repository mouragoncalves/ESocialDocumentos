using ESocial.Application.DTOs;
using ESocial.Application.UseCases.ConsultarIdentificadores;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ESocial.Api.Controllers;

[ApiController]
[Route("api/identificadores")]
public class IdentificadoresController : ControllerBase
{
    private readonly IMediator _mediator;

    public IdentificadoresController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Consulta identificadores de eventos no eSocial.
    /// GET /api/identificadores
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ConsultarIdentificadoresResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ConsultarIdentificadores(
        [FromQuery] ConsultaIdentificadoresDto consulta,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ConsultarIdentificadoresQuery(consulta), cancellationToken);
        return Ok(result);
    }
}
