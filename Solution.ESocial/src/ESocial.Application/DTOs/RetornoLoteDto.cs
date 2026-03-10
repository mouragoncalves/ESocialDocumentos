namespace ESocial.Application.DTOs;

public record RetornoLoteDto(
    string? Protocolo,
    string CdResposta,
    string DescResposta,
    bool Sucesso,
    IReadOnlyList<RetornoEventoDto>? Eventos = null
);

public record RetornoEventoDto(
    string Id,
    string CdResposta,
    string DescResposta
);
