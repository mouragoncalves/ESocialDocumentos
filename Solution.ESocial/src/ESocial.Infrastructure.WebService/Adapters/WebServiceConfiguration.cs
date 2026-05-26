using ESocial.Domain.Enums;

namespace ESocial.Infrastructure.WebService.Adapters;

public class WebServiceConfiguration
{
    // Envio de lote
    private const string EnvioLoteProducaoDefault = "https://webservices.producao.esocial.gov.br/servicos/empregador/envio/lote/v1_1_0/ServicoEnviarLoteEventos.svc";
    private const string EnvioLoteHomologacaoDefault = "https://webservices.homologacao.esocial.gov.br/servicos/empregador/envio/lote/v1_1_0/ServicoEnviarLoteEventos.svc";

    // Consulta de lote — domínio próprio: webservices.consulta.esocial.gov.br
    private const string ConsultaLoteProducaoDefault = "https://webservices.consulta.esocial.gov.br/servicos/empregador/consultarloteeventos/WsConsultarLoteEventos.svc";
    private const string ConsultaLoteHomologacaoDefault = "https://webservices.consulta.esocial.gov.br/servicos/empregador/consultarloteeventos/WsConsultarLoteEventos.svc";

    // Consulta de identificadores
    private const string ConsultaIdentificadoresProducaoDefault = "https://webservices.producao.esocial.gov.br/servicos/empregador/consulta/identificadores/v1_0_0/ServicoConsultarIdentificadoresEventos.svc";
    private const string ConsultaIdentificadoresHomologacaoDefault = "https://webservices.homologacao.esocial.gov.br/servicos/empregador/consulta/identificadores/v1_0_0/ServicoConsultarIdentificadoresEventos.svc";

    // Download
    private const string DownloadProducaoDefault = "https://webservices.producao.esocial.gov.br/servicos/empregador/download/solicitacao/v1_0_0/ServicoSolicitarDownloadEventos.svc";
    private const string DownloadHomologacaoDefault = "https://webservices.homologacao.esocial.gov.br/servicos/empregador/download/solicitacao/v1_0_0/ServicoSolicitarDownloadEventos.svc";

    public string? UrlEnvioLoteProducao { get; set; }
    public string? UrlEnvioLoteHomologacao { get; set; }

    public string? UrlConsultaLoteProducao { get; set; }
    public string? UrlConsultaLoteHomologacao { get; set; }

    public string? UrlConsultaIdentificadoresProducao { get; set; }
    public string? UrlConsultaIdentificadoresHomologacao { get; set; }

    public string? UrlDownloadProducao { get; set; }
    public string? UrlDownloadHomologacao { get; set; }

    /// <summary>
    /// Desabilita a validação do certificado SSL do servidor.
    /// Use apenas em ambiente de desenvolvimento quando os certificados ICP-Brasil
    /// não estiverem instalados no sistema operacional.
    /// NUNCA ative em produção.
    /// </summary>
    public bool IgnorarValidacaoSslServidor { get; set; } = false;

    public string ObterUrlEnvioLote(AmbienteEnvio ambiente) => Resolver(
        ambiente, UrlEnvioLoteProducao, UrlEnvioLoteHomologacao,
        EnvioLoteProducaoDefault, EnvioLoteHomologacaoDefault);

    public string ObterUrlConsultaLote(AmbienteEnvio ambiente) => Resolver(
        ambiente, UrlConsultaLoteProducao, UrlConsultaLoteHomologacao,
        ConsultaLoteProducaoDefault, ConsultaLoteHomologacaoDefault);

    public string ObterUrlConsultaIdentificadores(AmbienteEnvio ambiente) => Resolver(
        ambiente, UrlConsultaIdentificadoresProducao, UrlConsultaIdentificadoresHomologacao,
        ConsultaIdentificadoresProducaoDefault, ConsultaIdentificadoresHomologacaoDefault);

    public string ObterUrlDownload(AmbienteEnvio ambiente) => Resolver(
        ambiente, UrlDownloadProducao, UrlDownloadHomologacao,
        DownloadProducaoDefault, DownloadHomologacaoDefault);

    private static string Resolver(AmbienteEnvio ambiente, string? urlProd, string? urlHom, string defaultProd, string defaultHom)
    {
        var url = ambiente == AmbienteEnvio.Producao ? urlProd : urlHom;
        return string.IsNullOrWhiteSpace(url)
            ? (ambiente == AmbienteEnvio.Producao ? defaultProd : defaultHom)
            : url.TrimEnd('/');
    }
}
