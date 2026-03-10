using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace ESocial.Infrastructure.WebService.Generated;

// Contratos gerados manualmente com base no WSDL WsEnviarLoteEventos-v1_1_0.wsdl
// Namespace: http://www.esocial.gov.br/servicos/empregador/lote/eventos/envio/v1_1_0

[ServiceContract(
    Namespace = "http://www.esocial.gov.br/servicos/empregador/lote/eventos/envio/v1_1_0",
    Name = "ServicoEnviarLoteEventos")]
public interface IServicoEnviarLoteEventos
{
    [OperationContract(
        Action = "http://www.esocial.gov.br/servicos/empregador/lote/eventos/envio/v1_1_0/ServicoEnviarLoteEventos/EnviarLoteEventos",
        ReplyAction = "http://www.esocial.gov.br/servicos/empregador/lote/eventos/envio/v1_1_0/ServicoEnviarLoteEventos/EnviarLoteEventosResponse")]
    Task<Message> EnviarLoteEventosAsync(Message request);
}

public class WsEnviarLoteEventosClient : ClientBase<IServicoEnviarLoteEventos>, IServicoEnviarLoteEventos
{
    public WsEnviarLoteEventosClient(Binding binding, EndpointAddress address)
        : base(binding, address) { }

    public Task<Message> EnviarLoteEventosAsync(Message request)
        => Channel.EnviarLoteEventosAsync(request);
}
