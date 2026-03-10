using ESocial.Application.DTOs;

namespace ESocial.Application.UseCases.SolicitarDownload;

public record SolicitarDownloadResult(
    bool Sucesso,
    string CdResposta,
    string DescResposta,
    IReadOnlyList<ArquivoDownloadDto> Arquivos
);
