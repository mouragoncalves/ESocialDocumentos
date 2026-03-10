namespace ESocial.Domain.ValueObjects;

public record StatusProcessamento
{
    public string CdResposta { get; }
    public string DescResposta { get; }

    public StatusProcessamento(string cdResposta, string descResposta)
    {
        CdResposta = cdResposta ?? throw new ArgumentNullException(nameof(cdResposta));
        DescResposta = descResposta ?? throw new ArgumentNullException(nameof(descResposta));
    }

    public bool Sucesso => CdResposta == "201" || CdResposta == "202";

    public override string ToString() => $"[{CdResposta}] {DescResposta}";
}
