using ESocial.Domain.Enums;

namespace ESocial.Application.DTOs;

public record LoteDto(
    Guid EmpregadorId,
    int NumeroLote,
    GrupoEvento Grupo,
    AmbienteEnvio Ambiente,
    IReadOnlyList<EventoDto> Eventos
);
