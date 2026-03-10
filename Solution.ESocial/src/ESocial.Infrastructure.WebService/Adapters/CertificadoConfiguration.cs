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
            return new X509Certificate2(CaminhoArquivoPfx, SenhaPfx,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

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
