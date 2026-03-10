namespace ESocial.Application.UseCases.EnviarLote;

public record EnviarLoteResult(
    Guid LoteId,
    string? Protocolo,
    bool Sucesso,
    string CdResposta,
    string DescResposta
);
