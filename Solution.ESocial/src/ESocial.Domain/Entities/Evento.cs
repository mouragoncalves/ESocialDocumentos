using ESocial.Domain.ValueObjects;

namespace ESocial.Domain.Entities;

public class Evento
{
    public Guid Id { get; private set; }
    public string TipoEvento { get; private set; }
    public string XmlContent { get; private set; }
    public StatusProcessamento? Retorno { get; private set; }
    public DateTime CriadoEm { get; private set; }

    private Evento() { }

    public Evento(string tipoEvento, string xmlContent)
    {
        Id = Guid.NewGuid();
        TipoEvento = string.IsNullOrWhiteSpace(tipoEvento)
            ? throw new ArgumentException("Tipo de evento é obrigatório.", nameof(tipoEvento))
            : tipoEvento;
        XmlContent = string.IsNullOrWhiteSpace(xmlContent)
            ? throw new ArgumentException("Conteúdo XML é obrigatório.", nameof(xmlContent))
            : xmlContent;
        CriadoEm = DateTime.UtcNow;
    }

    public void RegistrarRetorno(StatusProcessamento retorno)
    {
        Retorno = retorno ?? throw new ArgumentNullException(nameof(retorno));
    }
}
