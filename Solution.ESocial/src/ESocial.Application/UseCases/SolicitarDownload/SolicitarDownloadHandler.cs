using ESocial.Application.Interfaces;
using MediatR;

namespace ESocial.Application.UseCases.SolicitarDownload;

public class SolicitarDownloadHandler : IRequestHandler<SolicitarDownloadCommand, SolicitarDownloadResult>
{
    private readonly IESocialWebService _webService;

    public SolicitarDownloadHandler(IESocialWebService webService)
    {
        _webService = webService;
    }

    public async Task<SolicitarDownloadResult> Handle(SolicitarDownloadCommand request, CancellationToken cancellationToken)
    {
        var retorno = await _webService.SolicitarDownloadAsync(request.Solicitacao, cancellationToken);
        return new SolicitarDownloadResult(retorno.Sucesso, retorno.CdResposta, retorno.DescResposta, retorno.Arquivos);
    }
}
