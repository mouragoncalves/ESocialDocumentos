using ESocial.Domain.Enums;
using MediatR;

namespace ESocial.Application.UseCases.ConsultarLote;

public record ConsultarLoteQuery(string Protocolo, AmbienteEnvio Ambiente) : IRequest<ConsultarLoteResult>;
