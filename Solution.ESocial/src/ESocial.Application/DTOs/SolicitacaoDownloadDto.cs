using ESocial.Domain.Enums;

namespace ESocial.Application.DTOs;

public enum TipoDownload
{
    PorId,
    PorNrRecibo
}

public record SolicitacaoDownloadDto(
    TipoDownload Tipo,
    string TipoInscricaoEmpregador,
    string NrInscricaoEmpregador,
    AmbienteEnvio Ambiente,
    IReadOnlyList<string> Identificadores
);

public record RetornoDownloadDto(
    string CdResposta,
    string DescResposta,
    bool Sucesso,
    IReadOnlyList<ArquivoDownloadDto> Arquivos
);

public record ArquivoDownloadDto(
    string Id,
    string XmlContent
);
