using ESocial.Application.Interfaces;
using ESocial.Domain.Repositories;
using ESocial.Domain.ValueObjects;
using MediatR;

namespace ESocial.Application.UseCases.ConsultarLote;

public class ConsultarLoteHandler : IRequestHandler<ConsultarLoteQuery, ConsultarLoteResult>
{
    private readonly IESocialWebService _webService;
    private readonly ILoteEventosRepository _loteRepository;

    public ConsultarLoteHandler(IESocialWebService webService, ILoteEventosRepository loteRepository)
    {
        _webService = webService;
        _loteRepository = loteRepository;
    }

    public async Task<ConsultarLoteResult> Handle(ConsultarLoteQuery request, CancellationToken cancellationToken)
    {
        var retorno = await _webService.ConsultarLoteEventosAsync(request.Protocolo, request.Ambiente, cancellationToken);

        var lote = await _loteRepository.ObterPorProtocoloAsync(
            new ProtocoloEnvio(request.Protocolo), cancellationToken);

        if (lote is not null && retorno.Sucesso)
        {
            lote.MarcarComoProcessado(new StatusProcessamento(retorno.CdResposta, retorno.DescResposta));
            await _loteRepository.AtualizarAsync(lote, cancellationToken);
            await _loteRepository.SaveChangesAsync(cancellationToken);
        }

        return new ConsultarLoteResult(
            request.Protocolo,
            retorno.CdResposta,
            retorno.DescResposta,
            retorno.Sucesso,
            retorno.Eventos);
    }
}
