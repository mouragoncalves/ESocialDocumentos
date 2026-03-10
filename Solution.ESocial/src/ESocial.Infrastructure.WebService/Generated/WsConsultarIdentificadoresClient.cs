using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ESocial.Infrastructure.WebService.Generated;

// Contratos gerados manualmente com base no WSDL WsConsultarIdentificadoresEventos-v1_0_0.wsdl
// Namespace: http://www.esocial.gov.br/servicos/empregador/eventos/identificadores/consulta/v1_0_0

[ServiceContract(
    Namespace = "http://www.esocial.gov.br/servicos/empregador/eventos/identificadores/consulta/v1_0_0",
    Name = "ServicoConsultarIdentificadoresEventos")]
public interface IServicoConsultarIdentificadoresEventos
{
    [OperationContract(
        Action = "http://www.esocial.gov.br/servicos/empregador/eventos/identificadores/consulta/v1_0_0/ServicoConsultarIdentificadoresEventos/ConsultarIdentificadoresEmpregador",
        ReplyAction = "*")]
    Task<Message> ConsultarIdentificadoresEmpregadorAsync(Message request);

    [OperationContract(
        Action = "http://www.esocial.gov.br/servicos/empregador/eventos/identificadores/consulta/v1_0_0/ServicoConsultarIdentificadoresEventos/ConsultarIdentificadoresTrabalhador",
        ReplyAction = "*")]
    Task<Message> ConsultarIdentificadoresTrabalhadorAsync(Message request);

    [OperationContract(
        Action = "http://www.esocial.gov.br/servicos/empregador/eventos/identificadores/consulta/v1_0_0/ServicoConsultarIdentificadoresEventos/ConsultarIdentificadoresTabela",
        ReplyAction = "*")]
    Task<Message> ConsultarIdentificadoresTabelaAsync(Message request);
}

public class WsConsultarIdentificadoresClient : ClientBase<IServicoConsultarIdentificadoresEventos>, IServicoConsultarIdentificadoresEventos
{
    public WsConsultarIdentificadoresClient(Binding binding, EndpointAddress address)
        : base(binding, address) { }

    public Task<Message> ConsultarIdentificadoresEmpregadorAsync(Message request)
        => Channel.ConsultarIdentificadoresEmpregadorAsync(request);

    public Task<Message> ConsultarIdentificadoresTrabalhadorAsync(Message request)
        => Channel.ConsultarIdentificadoresTrabalhadorAsync(request);

    public Task<Message> ConsultarIdentificadoresTabelaAsync(Message request)
        => Channel.ConsultarIdentificadoresTabelaAsync(request);
}
