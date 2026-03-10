namespace ESocial.Domain.ValueObjects;

/// <summary>
/// Protocolo de envio no formato A.B.AAAAMM.NNNNN (ex: 1.2.202503.12345)
/// </summary>
public record ProtocoloEnvio
{
    public string Valor { get; }

    public ProtocoloEnvio(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Protocolo de envio não pode ser vazio.", nameof(valor));

        Valor = valor;
    }

    public override string ToString() => Valor;

    public static implicit operator string(ProtocoloEnvio p) => p.Valor;
    public static explicit operator ProtocoloEnvio(string s) => new(s);
}
