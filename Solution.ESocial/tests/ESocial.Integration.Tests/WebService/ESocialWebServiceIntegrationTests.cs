using ESocial.Application.DTOs;
using ESocial.Domain.Enums;
using ESocial.Infrastructure.WebService.Adapters;
using FluentAssertions;

namespace ESocial.Integration.Tests.WebService;

/// <summary>
/// Testes de integração contra o webservice de homologação do eSocial.
/// Requerem certificado digital válido configurado nas variáveis de ambiente.
///
/// Para executar, defina:
///   ESOCIAL_CERT_PATH   — caminho para o arquivo .pfx do certificado
///   ESOCIAL_CERT_PASS   — senha do certificado
/// </summary>
[Trait("Category", "Integration")]
public class ESocialWebServiceIntegrationTests
{
    private static ESocialWebServiceAdapter? CriarAdapter()
    {
        var certPath = Environment.GetEnvironmentVariable("ESOCIAL_CERT_PATH");
        var certPass = Environment.GetEnvironmentVariable("ESOCIAL_CERT_PASS");

        if (string.IsNullOrWhiteSpace(certPath) || !File.Exists(certPath))
            return null;

        return new ESocialWebServiceAdapter(new CertificadoConfiguration
        {
            CaminhoArquivoPfx = certPath,
            SenhaPfx = certPass
        });
    }

    [Fact]
    public async Task EnviarLote_Homologacao_DeveRetornarProtocolo()
    {
        var adapter = CriarAdapter();
        if (adapter is null)
        {
            // Pula o teste se o certificado não estiver configurado.
            return;
        }

        var loteDto = new LoteDto(
            Guid.NewGuid(), 1, GrupoEvento.Tabela, AmbienteEnvio.Homologacao,
            []);

        var retorno = await adapter.EnviarLoteEventosAsync(loteDto, CancellationToken.None);
        retorno.Should().NotBeNull();
    }
}
