namespace ESocial.Application.DTOs;

public record RetornoLoteDto(
    string? Protocolo,
    string CdResposta,
    string DescResposta,
    bool Sucesso,
    IReadOnlyList<RetornoEventoDto>? Eventos = null,
    IReadOnlyList<OcorrenciaDto>? Ocorrencias = null,
    int? TempoEstimadoConclusao = null
);

public record RetornoEventoDto(
    string Id,
    string CdResposta,
    string DescResposta,
    IReadOnlyList<OcorrenciaDto>? Ocorrencias = null
);

public record OcorrenciaDto(
    int Codigo,
    string Descricao,
    byte Tipo,
    string? Localizacao = null
);
