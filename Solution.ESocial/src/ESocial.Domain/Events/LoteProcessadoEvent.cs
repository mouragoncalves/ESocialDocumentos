using ESocial.Domain.ValueObjects;

namespace ESocial.Domain.Events;

public record LoteProcessadoEvent(Guid LoteId, string Protocolo, StatusProcessamento Status);
