using ESocial.Domain.Entities;
using ESocial.Domain.ValueObjects;

namespace ESocial.Domain.Repositories;

public interface IEmpregadorRepository
{
    Task<Empregador?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Empregador?> ObterPorInscricaoAsync(Inscricao inscricao, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Empregador empregador, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Empregador empregador, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
