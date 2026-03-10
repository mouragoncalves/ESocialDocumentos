using ESocial.Domain.ValueObjects;

namespace ESocial.Domain.Entities;

public class Transmissor
{
    public Guid Id { get; private set; }
    public Inscricao Inscricao { get; private set; }
    public string NomeRazaoSocial { get; private set; }

    private Transmissor() { }

    public Transmissor(Inscricao inscricao, string nomeRazaoSocial)
    {
        Id = Guid.NewGuid();
        Inscricao = inscricao ?? throw new ArgumentNullException(nameof(inscricao));
        NomeRazaoSocial = string.IsNullOrWhiteSpace(nomeRazaoSocial)
            ? throw new ArgumentException("Nome/Razão Social é obrigatório.", nameof(nomeRazaoSocial))
            : nomeRazaoSocial;
    }
}
