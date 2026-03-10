using System.Security.Cryptography.X509Certificates;

namespace ESocial.Infrastructure.WebService.Adapters;

public class CertificadoConfiguration
{
    public string? Thumbprint { get; set; }
    public string? CaminhoArquivoPfx { get; set; }
    public string? SenhaPfx { get; set; }
    public StoreLocation StoreLocation { get; set; } = StoreLocation.CurrentUser;
    public StoreName StoreName { get; set; } = StoreName.My;

    public X509Certificate2 CarregarCertificado()
    {
        if (!string.IsNullOrWhiteSpace(CaminhoArquivoPfx))
        {
            if (!File.Exists(CaminhoArquivoPfx))
                throw new InvalidOperationException(
                    $"Arquivo de certificado não encontrado: '{Path.GetFullPath(CaminhoArquivoPfx)}'. " +
                    "Verifique o caminho em ESocial:Certificado:CaminhoArquivoPfx no appsettings.");

            try
            {
                return X509CertificateLoader.LoadPkcs12FromFile(
                    CaminhoArquivoPfx, SenhaPfx,
                    X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            }
            catch (System.Security.Cryptography.CryptographicException ex)
            {
                throw new InvalidOperationException(
                    $"Não foi possível carregar o certificado '{CaminhoArquivoPfx}': senha incorreta ou arquivo corrompido. " +
                    "Verifique ESocial:Certificado:SenhaPfx no appsettings.", ex);
            }
        }

        if (!string.IsNullOrWhiteSpace(Thumbprint))
        {
            using var store = new X509Store(StoreName, StoreLocation);
            store.Open(OpenFlags.ReadOnly);
            var certs = store.Certificates.Find(X509FindType.FindByThumbprint, Thumbprint, false);
            if (certs.Count == 0)
                throw new InvalidOperationException($"Certificado com thumbprint '{Thumbprint}' não encontrado.");
            return certs[0];
        }

        throw new InvalidOperationException("Nenhuma configuração de certificado foi fornecida.");
    }
}
