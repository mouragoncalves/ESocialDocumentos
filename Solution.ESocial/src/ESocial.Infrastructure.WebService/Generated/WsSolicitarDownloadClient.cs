using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ESocial.Infrastructure.WebService.Generated;

// Contratos gerados manualmente com base no WSDL WsSolicitarDownloadEventos-v1_0_0.wsdl
// Namespace: http://www.esocial.gov.br/servicos/empregador/eventos/download/solicitacao/v1_0_0

[ServiceContract(
    Namespace = "http://www.esocial.gov.br/servicos/empregador/eventos/download/solicitacao/v1_0_0",
    Name = "ServicoSolicitarDownloadEventos")]
public interface IServicoSolicitarDownloadEventos
{
    [OperationContract(
        Action = "http://www.esocial.gov.br/servicos/empregador/eventos/download/solicitacao/v1_0_0/ServicoSolicitarDownloadEventos/SolicitarDownloadEventosPorId",
        ReplyAction = "*")]
    Task<Message> SolicitarDownloadEventosPorIdAsync(Message request);

    [OperationContract(
        Action = "http://www.esocial.gov.br/servicos/empregador/eventos/download/solicitacao/v1_0_0/ServicoSolicitarDownloadEventos/SolicitarDownloadEventosPorNrRecibo",
        ReplyAction = "*")]
    Task<Message> SolicitarDownloadEventosPorNrReciboAsync(Message request);
}

public class WsSolicitarDownloadClient : ClientBase<IServicoSolicitarDownloadEventos>, IServicoSolicitarDownloadEventos
{
    public WsSolicitarDownloadClient(Binding binding, EndpointAddress address)
        : base(binding, address) { }

    public Task<Message> SolicitarDownloadEventosPorIdAsync(Message request)
        => Channel.SolicitarDownloadEventosPorIdAsync(request);

    public Task<Message> SolicitarDownloadEventosPorNrReciboAsync(Message request)
        => Channel.SolicitarDownloadEventosPorNrReciboAsync(request);
}
