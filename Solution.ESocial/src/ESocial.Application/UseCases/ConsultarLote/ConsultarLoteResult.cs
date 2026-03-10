using ESocial.Application.DTOs;

namespace ESocial.Application.UseCases.ConsultarLote;

public record ConsultarLoteResult(
    string Protocolo,
    string CdResposta,
    string DescResposta,
    bool Sucesso,
    IReadOnlyList<RetornoEventoDto>? Eventos
);
