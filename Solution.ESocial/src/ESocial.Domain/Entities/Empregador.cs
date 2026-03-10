using ESocial.Domain.ValueObjects;

namespace ESocial.Domain.Entities;

/// <summary>
/// Aggregate Root: representa o empregador que envia eventos ao eSocial.
/// </summary>
public class Empregador
{
    public Guid Id { get; private set; }
    public Inscricao Inscricao { get; private set; }
    public string NomeRazaoSocial { get; private set; }

    private readonly List<LoteEventos> _lotes = [];
    public IReadOnlyCollection<LoteEventos> Lotes => _lotes.AsReadOnly();

    private Empregador() { }

    public Empregador(Inscricao inscricao, string nomeRazaoSocial)
    {
        Id = Guid.NewGuid();
        Inscricao = inscricao ?? throw new ArgumentNullException(nameof(inscricao));
        NomeRazaoSocial = string.IsNullOrWhiteSpace(nomeRazaoSocial)
            ? throw new ArgumentException("Nome/Razão Social é obrigatório.", nameof(nomeRazaoSocial))
            : nomeRazaoSocial;
    }

    public void AtualizarNome(string novoNome)
    {
        if (string.IsNullOrWhiteSpace(novoNome))
            throw new ArgumentException("Nome não pode ser vazio.", nameof(novoNome));
        NomeRazaoSocial = novoNome;
    }
}
