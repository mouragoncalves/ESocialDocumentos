using ESocial.Application.DTOs;
using MediatR;

namespace ESocial.Application.UseCases.EnviarLote;

public record EnviarLoteCommand(LoteDto Lote) : IRequest<EnviarLoteResult>;
