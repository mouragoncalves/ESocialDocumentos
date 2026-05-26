using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Xml;
using System.Xml.Linq;
using ESocial.Application.DTOs;
using ESocial.Application.Interfaces;
using ESocial.Domain.Enums;
using ESocial.Infrastructure.WebService.Generated;

namespace ESocial.Infrastructure.WebService.Adapters;

/// <summary>
/// Implementa IESocialWebService, configurando mTLS (X.509) e serializando/desserializando XML bruto.
/// Endereços dos webservices eSocial v1.6:
///   Produção  Envio:    https://webservices.producao.esocial.gov.br/servicos/empregador/envio/lote/v1_1_0/ServicoEnviarLoteEventos.svc
///   Produção  Consulta: https://webservices.producao.esocial.gov.br/servicos/empregador/consulta/lote/v1_1_0/ServicoConsultarLoteEventos.svc
///   Homolog.  Envio:    https://webservices.homologacao.esocial.gov.br/servicos/empregador/envio/lote/v1_1_0/ServicoEnviarLoteEventos.svc
///   Homolog.  Consulta: https://webservices.homologacao.esocial.gov.br/servicos/empregador/consulta/lote/v1_1_0/ServicoConsultarLoteEventos.svc
/// </summary>
public class ESocialWebServiceAdapter : IESocialWebService
{
    private readonly CertificadoConfiguration _certConfig;

    // URLs base por ambiente
    private static readonly Dictionary<AmbienteEnvio, string> _baseUrls = new()
    {
        [AmbienteEnvio.Producao] = "https://webservices.producao.esocial.gov.br/servicos/empregador",
        [AmbienteEnvio.Homologacao] = "https://webservices.homologacao.esocial.gov.br/servicos/empregador"
    };

    public ESocialWebServiceAdapter(CertificadoConfiguration certConfig)
    {
        _certConfig = certConfig ?? throw new ArgumentNullException(nameof(certConfig));
    }

    public async Task<RetornoLoteDto> EnviarLoteEventosAsync(LoteDto lote, CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrls[lote.Ambiente]}/envio/lote/v1_1_0/ServicoEnviarLoteEventos.svc";
        var binding = CriarBinding();
        var client = new WsEnviarLoteEventosClient(binding, new EndpointAddress(url));
        ConfigurarCertificado(client.ClientCredentials);

        var xmlEnvio = MontarXmlEnvioLote(lote);
        var requestMsg = Message.CreateMessage(
            MessageVersion.Soap11,
            "http://www.esocial.gov.br/servicos/empregador/lote/eventos/envio/v1_1_0/ServicoEnviarLoteEventos/EnviarLoteEventos",
            new XmlBodyWriter(xmlEnvio));

        var responseMsg = await client.EnviarLoteEventosAsync(requestMsg);
        return ParseRetornoEnvio(responseMsg);
    }

    public async Task<RetornoLoteDto> ConsultarLoteEventosAsync(string protocolo, AmbienteEnvio ambiente, CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrls[ambiente]}/consulta/lote/v1_1_0/ServicoConsultarLoteEventos.svc";
        var binding = CriarBinding();
        var client = new WsConsultarLoteEventosClient(binding, new EndpointAddress(url));
        ConfigurarCertificado(client.ClientCredentials);

        var xmlConsulta = MontarXmlConsultaLote(protocolo, ambiente);
        var requestMsg = Message.CreateMessage(
            MessageVersion.Soap11,
            "http://www.esocial.gov.br/servicos/empregador/lote/eventos/envio/consulta/retornoProcessamento/v1_1_0/ServicoConsultarLoteEventos/ConsultarLoteEventos",
            new XmlBodyWriter(xmlConsulta));

        var responseMsg = await client.ConsultarLoteEventosAsync(requestMsg);
        return ParseRetornoConsulta(responseMsg);
    }

    public async Task<IReadOnlyList<string>> ConsultarIdentificadoresAsync(ConsultaIdentificadoresDto consulta, CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrls[consulta.Ambiente]}/consulta/identificadores/v1_0_0/ServicoConsultarIdentificadoresEventos.svc";
        var binding = CriarBinding();
        var client = new WsConsultarIdentificadoresClient(binding, new EndpointAddress(url));
        ConfigurarCertificado(client.ClientCredentials);

        var xmlConsulta = MontarXmlConsultaIdentificadores(consulta);
        Message responseMsg;

        var action = consulta.Tipo switch
        {
            TipoConsultaIdentificadores.Empregador => "ConsultarIdentificadoresEmpregador",
            TipoConsultaIdentificadores.Trabalhador => "ConsultarIdentificadoresTrabalhador",
            TipoConsultaIdentificadores.Tabela => "ConsultarIdentificadoresTabela",
            _ => throw new ArgumentException("Tipo de consulta inválido.")
        };

        var requestMsg = Message.CreateMessage(
            MessageVersion.Soap11,
            $"http://www.esocial.gov.br/servicos/empregador/eventos/identificadores/consulta/v1_0_0/ServicoConsultarIdentificadoresEventos/{action}",
            new XmlBodyWriter(xmlConsulta));

        responseMsg = consulta.Tipo switch
        {
            TipoConsultaIdentificadores.Empregador => await client.ConsultarIdentificadoresEmpregadorAsync(requestMsg),
            TipoConsultaIdentificadores.Trabalhador => await client.ConsultarIdentificadoresTrabalhadorAsync(requestMsg),
            TipoConsultaIdentificadores.Tabela => await client.ConsultarIdentificadoresTabelaAsync(requestMsg),
            _ => throw new ArgumentException("Tipo de consulta inválido.")
        };

        return ParseIdentificadores(responseMsg);
    }

    public async Task<RetornoDownloadDto> SolicitarDownloadAsync(SolicitacaoDownloadDto solicitacao, CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrls[solicitacao.Ambiente]}/download/solicitacao/v1_0_0/ServicoSolicitarDownloadEventos.svc";
        var binding = CriarBinding();
        var client = new WsSolicitarDownloadClient(binding, new EndpointAddress(url));
        ConfigurarCertificado(client.ClientCredentials);

        var xmlSolicitacao = MontarXmlSolicitacaoDownload(solicitacao);
        var action = solicitacao.Tipo == TipoDownload.PorId
            ? "SolicitarDownloadEventosPorId"
            : "SolicitarDownloadEventosPorNrRecibo";

        var requestMsg = Message.CreateMessage(
            MessageVersion.Soap11,
            $"http://www.esocial.gov.br/servicos/empregador/eventos/download/solicitacao/v1_0_0/ServicoSolicitarDownloadEventos/{action}",
            new XmlBodyWriter(xmlSolicitacao));

        var responseMsg = solicitacao.Tipo == TipoDownload.PorId
            ? await client.SolicitarDownloadEventosPorIdAsync(requestMsg)
            : await client.SolicitarDownloadEventosPorNrReciboAsync(requestMsg);

        return ParseRetornoDownload(responseMsg);
    }

    // --- Helpers ---

    private static BasicHttpsBinding CriarBinding()
        => new BasicHttpsBinding(BasicHttpsSecurityMode.Transport)
        {
            MaxReceivedMessageSize = 10 * 1024 * 1024, // 10 MB
            SendTimeout = TimeSpan.FromSeconds(60),
            ReceiveTimeout = TimeSpan.FromSeconds(60)
        };

    private void ConfigurarCertificado(System.ServiceModel.Description.ClientCredentials? credentials)
    {
        if (credentials is null) return;
        credentials.ClientCertificate.Certificate = _certConfig.CarregarCertificado();
        credentials.ServiceCertificate.Authentication.CertificateValidationMode =
            X509CertificateValidationMode.None;
    }

    private static string MontarXmlEnvioLote(LoteDto lote)
    {
        var ns = "http://www.esocial.gov.br/schema/lote/eventos/envio/v1_1_1";
        var doc = new XDocument(
            new XElement(XName.Get("eSocial", ns),
                new XElement(XName.Get("envioLoteEventos", ns),
                    new XAttribute("grupo", (int)lote.Grupo),
                    new XElement(XName.Get("ideEmpregador", ns),
                        new XElement(XName.Get("tpInsc", ns), 1),
                        new XElement(XName.Get("nrInsc", ns), "00000000000000")),
                    new XElement(XName.Get("ideTransmissor", ns),
                        new XElement(XName.Get("tpInsc", ns), 1),
                        new XElement(XName.Get("nrInsc", ns), "00000000000000")),
                    new XElement(XName.Get("eventos", ns),
                        lote.Eventos.Select((e, i) =>
                            new XElement(XName.Get("evento", ns),
                                new XAttribute("Id", $"ev{i + 1}"),
                                XElement.Parse(e.XmlContent)))))));
        return doc.ToString();
    }

    private static string MontarXmlConsultaLote(string protocolo, AmbienteEnvio ambiente)
    {
        var ns = "http://www.esocial.gov.br/schema/lote/eventos/envio/consulta/retornoProcessamento/v1_0_0";
        var doc = new XDocument(
            new XElement(XName.Get("eSocial", ns),
                new XElement(XName.Get("consultaLoteEventos", ns),
                    new XElement(XName.Get("protocoloEnvio", ns), protocolo))));
        return doc.ToString();
    }

    private static string MontarXmlConsultaIdentificadores(ConsultaIdentificadoresDto consulta)
    {
        var ns = "http://www.esocial.gov.br/schema/consulta/identificadores-eventos/empregador/v1_0_0";
        var doc = new XDocument(
            new XElement(XName.Get("eSocial", ns),
                new XElement(XName.Get("consultaIdentificadoresEvts", ns),
                    new XElement(XName.Get("ideEmpregador", ns),
                        new XElement(XName.Get("tpInsc", ns), consulta.TipoInscricaoEmpregador),
                        new XElement(XName.Get("nrInsc", ns), consulta.NrInscricaoEmpregador)))));
        return doc.ToString();
    }

    private static string MontarXmlSolicitacaoDownload(SolicitacaoDownloadDto solicitacao)
    {
        var ns = "http://www.esocial.gov.br/schema/download/solicitacao/id/v1_0_0";
        var elementoIds = solicitacao.Tipo == TipoDownload.PorId
            ? solicitacao.Identificadores.Select(id => new XElement(XName.Get("id", ns), id))
            : solicitacao.Identificadores.Select(id => new XElement(XName.Get("nrRec", ns), id));

        var doc = new XDocument(
            new XElement(XName.Get("eSocial", ns),
                new XElement(XName.Get("download", ns),
                    new XElement(XName.Get("ideEmpregador", ns),
                        new XElement(XName.Get("tpInsc", ns), solicitacao.TipoInscricaoEmpregador),
                        new XElement(XName.Get("nrInsc", ns), solicitacao.NrInscricaoEmpregador)),
                    new XElement(XName.Get("solicDownloadEvtsPorId", ns), elementoIds))));
        return doc.ToString();
    }

    private static RetornoLoteDto ParseRetornoEnvio(Message responseMsg)
    {
        var xml = LerXmlMensagem(responseMsg);
        var retorno = XElement.Parse(xml);
        var status = retorno.Descendants().FirstOrDefault(e => e.Name.LocalName == "status");
        var cdResp = status?.Element(XName.Get("cdResposta", status.Name.NamespaceName))?.Value
            ?? status?.Descendants().FirstOrDefault(e => e.Name.LocalName == "cdResposta")?.Value ?? "000";
        var descResp = status?.Descendants().FirstOrDefault(e => e.Name.LocalName == "descResposta")?.Value ?? "";
        var protocolo = retorno.Descendants().FirstOrDefault(e => e.Name.LocalName == "protocoloEnvio")?.Value;

        return new RetornoLoteDto(protocolo, cdResp, descResp, cdResp is "201" or "202");
    }

    private static RetornoLoteDto ParseRetornoConsulta(Message responseMsg)
    {
        var xml = LerXmlMensagem(responseMsg);
        var retorno = XElement.Parse(xml);

        var status = retorno.Descendants().FirstOrDefault(e => e.Name.LocalName == "status");
        var cdResp = status?.Descendants().FirstOrDefault(e => e.Name.LocalName == "cdResposta")?.Value ?? "000";
        var descResp = status?.Descendants().FirstOrDefault(e => e.Name.LocalName == "descResposta")?.Value ?? "";
        var tempoEstimado = status?.Descendants().FirstOrDefault(e => e.Name.LocalName == "tempoEstimadoConclusao")?.Value;
        var ocorrencias = ParseOcorrencias(status);

        // protocoloEnvio fica em dadosRecepcaoLote > protocoloEnvio
        var protocolo = retorno.Descendants().FirstOrDefault(e => e.Name.LocalName == "protocoloEnvio")?.Value;

        // Id está em <evento Id="evN">, não em <retornoEvento>
        var eventos = retorno.Descendants()
            .Where(e => e.Name.LocalName == "evento" && e.Attribute("Id") != null)
            .Select(e => new RetornoEventoDto(
                e.Attribute("Id")!.Value,
                e.Descendants().FirstOrDefault(x => x.Name.LocalName == "cdResposta")?.Value ?? "",
                e.Descendants().FirstOrDefault(x => x.Name.LocalName == "descResposta")?.Value ?? "",
                ParseOcorrencias(e)))
            .ToList();

        return new RetornoLoteDto(
            protocolo, cdResp, descResp,
            cdResp is "201" or "202",
            eventos, ocorrencias,
            tempoEstimado is null ? null : int.TryParse(tempoEstimado, out var t) ? t : null);
    }

    private static IReadOnlyList<OcorrenciaDto>? ParseOcorrencias(XElement? container)
    {
        if (container is null) return null;
        var lista = container.Descendants()
            .Where(e => e.Name.LocalName == "ocorrencia")
            .Select(o => new OcorrenciaDto(
                int.TryParse(o.Descendants().FirstOrDefault(x => x.Name.LocalName == "codigo")?.Value, out var cod) ? cod : 0,
                o.Descendants().FirstOrDefault(x => x.Name.LocalName == "descricao")?.Value ?? "",
                byte.TryParse(o.Descendants().FirstOrDefault(x => x.Name.LocalName == "tipo")?.Value, out var tipo) ? tipo : (byte)0,
                o.Descendants().FirstOrDefault(x => x.Name.LocalName == "localizacao")?.Value))
            .ToList();
        return lista.Count > 0 ? lista : null;
    }

    private static IReadOnlyList<string> ParseIdentificadores(Message responseMsg)
    {
        var xml = LerXmlMensagem(responseMsg);
        var retorno = XElement.Parse(xml);
        return retorno.Descendants()
            .Where(e => e.Name.LocalName == "nrRec")
            .Select(e => e.Value)
            .ToList()
            .AsReadOnly();
    }

    private static RetornoDownloadDto ParseRetornoDownload(Message responseMsg)
    {
        var xml = LerXmlMensagem(responseMsg);
        var retorno = XElement.Parse(xml);
        var status = retorno.Descendants().FirstOrDefault(e => e.Name.LocalName == "status");
        var cdResp = status?.Descendants().FirstOrDefault(e => e.Name.LocalName == "cdResposta")?.Value ?? "000";
        var descResp = status?.Descendants().FirstOrDefault(e => e.Name.LocalName == "descResposta")?.Value ?? "";

        var arquivos = retorno.Descendants()
            .Where(e => e.Name.LocalName == "evento")
            .Select(e => new ArquivoDownloadDto(
                e.Attribute("Id")?.Value ?? "",
                e.FirstNode?.ToString() ?? ""))
            .ToList();

        return new RetornoDownloadDto(cdResp, descResp, cdResp is "200", arquivos);
    }

    private static string LerXmlMensagem(Message msg)
    {
        using var reader = msg.GetReaderAtBodyContents();
        reader.MoveToContent();
        return reader.ReadOuterXml();
    }
}

/// <summary>Escreve XML bruto no corpo da mensagem SOAP.</summary>
file class XmlBodyWriter : BodyWriter
{
    private readonly string _xml;

    public XmlBodyWriter(string xml) : base(isBuffered: true)
    {
        _xml = xml;
    }

    protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
    {
        using var reader = XmlReader.Create(new StringReader(_xml));
        writer.WriteNode(reader, defattr: false);
    }
}
