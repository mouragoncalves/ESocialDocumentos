namespace ESocial.Domain.Services;

public interface IXmlAssinaturaService
{
    /// <summary>
    /// Assina o XML do evento com o certificado digital do empregador.
    /// </summary>
    /// <param name="xmlContent">Conteúdo XML do evento.</param>
    /// <returns>XML assinado.</returns>
    string Assinar(string xmlContent);

    /// <summary>
    /// Verifica se o XML possui assinatura digital válida.
    /// </summary>
    bool VerificarAssinatura(string xmlContent);
}
