using ESocial.Application.DTOs;
using MediatR;

namespace ESocial.Application.UseCases.SolicitarDownload;

public record SolicitarDownloadCommand(SolicitacaoDownloadDto Solicitacao) : IRequest<SolicitarDownloadResult>;
