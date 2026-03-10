using ESocial.Application.Interfaces;
using MediatR;

namespace ESocial.Application.UseCases.ConsultarIdentificadores;

public class ConsultarIdentificadoresHandler : IRequestHandler<ConsultarIdentificadoresQuery, ConsultarIdentificadoresResult>
{
    private readonly IESocialWebService _webService;

    public ConsultarIdentificadoresHandler(IESocialWebService webService)
    {
        _webService = webService;
    }

    public async Task<ConsultarIdentificadoresResult> Handle(ConsultarIdentificadoresQuery request, CancellationToken cancellationToken)
    {
        var identificadores = await _webService.ConsultarIdentificadoresAsync(request.Consulta, cancellationToken);
        return new ConsultarIdentificadoresResult(identificadores);
    }
}
