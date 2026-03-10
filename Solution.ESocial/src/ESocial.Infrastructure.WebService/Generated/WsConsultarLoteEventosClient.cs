using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ESocial.Infrastructure.WebService.Generated;

// Contratos gerados manualmente com base no WSDL WsConsultarLoteEventos-v1_1_0.wsdl
// Namespace: http://www.esocial.gov.br/servicos/empregador/lote/eventos/envio/consulta/retornoProcessamento/v1_1_0

[ServiceContract(
    Namespace = "http://www.esocial.gov.br/servicos/empregador/lote/eventos/envio/consulta/retornoProcessamento/v1_1_0",
    Name = "ServicoConsultarLoteEventos")]
public interface IServicoConsultarLoteEventos
{
    [OperationContract(
        Action = "http://www.esocial.gov.br/servicos/empregador/lote/eventos/envio/consulta/retornoProcessamento/v1_1_0/ServicoConsultarLoteEventos/ConsultarLoteEventos",
        ReplyAction = "http://www.esocial.gov.br/servicos/empregador/lote/eventos/envio/consulta/retornoProcessamento/v1_1_0/ServicoConsultarLoteEventos/ConsultarLoteEventosResponse")]
    Task<Message> ConsultarLoteEventosAsync(Message request);
}

public class WsConsultarLoteEventosClient : ClientBase<IServicoConsultarLoteEventos>, IServicoConsultarLoteEventos
{
    public WsConsultarLoteEventosClient(Binding binding, EndpointAddress address)
        : base(binding, address) { }

    public Task<Message> ConsultarLoteEventosAsync(Message request)
        => Channel.ConsultarLoteEventosAsync(request);
}
