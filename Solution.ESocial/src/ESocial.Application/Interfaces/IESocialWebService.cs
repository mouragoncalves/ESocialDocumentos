using ESocial.Application.DTOs;
using ESocial.Domain.Enums;

namespace ESocial.Application.Interfaces;

/// <summary>
/// Abstração das 4 operações principais do webservice eSocial v1.6.
/// </summary>
public interface IESocialWebService
{
    /// <summary>WsEnviarLoteEventos — envia um lote de eventos.</summary>
    Task<RetornoLoteDto> EnviarLoteEventosAsync(LoteDto lote, CancellationToken cancellationToken = default);

    /// <summary>WsConsultarLoteEventos — consulta o resultado de processamento de um lote.</summary>
    Task<RetornoLoteDto> ConsultarLoteEventosAsync(string protocolo, AmbienteEnvio ambiente, CancellationToken cancellationToken = default);

    /// <summary>WsConsultarIdentificadoresEventos — consulta identificadores de eventos por empregador, tabela ou trabalhador.</summary>
    Task<IReadOnlyList<string>> ConsultarIdentificadoresAsync(ConsultaIdentificadoresDto consulta, CancellationToken cancellationToken = default);

    /// <summary>WsSolicitarDownloadEventos — solicita download de eventos por ID ou nrRecibo.</summary>
    Task<RetornoDownloadDto> SolicitarDownloadAsync(SolicitacaoDownloadDto solicitacao, CancellationToken cancellationToken = default);
}
