using ESocial.Domain.Entities;
using ESocial.Domain.ValueObjects;

namespace ESocial.Domain.Repositories;

public interface ILoteEventosRepository
{
    Task<LoteEventos?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LoteEventos?> ObterPorProtocoloAsync(ProtocoloEnvio protocolo, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoteEventos>> ListarPorEmpregadorAsync(Guid empregadorId, CancellationToken cancellationToken = default);
    Task AdicionarAsync(LoteEventos lote, CancellationToken cancellationToken = default);
    Task AtualizarAsync(LoteEventos lote, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
