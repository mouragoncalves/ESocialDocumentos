using ESocial.Domain.Entities;
using ESocial.Domain.Repositories;
using ESocial.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ESocial.Infrastructure.Persistence.Repositories;

public class LoteEventosRepository : ILoteEventosRepository
{
    private readonly ESocialDbContext _context;

    public LoteEventosRepository(ESocialDbContext context)
    {
        _context = context;
    }

    public async Task<LoteEventos?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.LotesEventos
            .Include(l => l.Eventos)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public async Task<LoteEventos?> ObterPorProtocoloAsync(ProtocoloEnvio protocolo, CancellationToken cancellationToken = default)
        => await _context.LotesEventos
            .Include(l => l.Eventos)
            .FirstOrDefaultAsync(l => l.Protocolo != null && l.Protocolo.Valor == protocolo.Valor, cancellationToken);

    public async Task<IEnumerable<LoteEventos>> ListarPorEmpregadorAsync(Guid empregadorId, CancellationToken cancellationToken = default)
        => await _context.LotesEventos
            .Include(l => l.Eventos)
            .Where(l => l.EmpregadorId == empregadorId)
            .OrderByDescending(l => l.CriadoEm)
            .ToListAsync(cancellationToken);

    public async Task AdicionarAsync(LoteEventos lote, CancellationToken cancellationToken = default)
        => await _context.LotesEventos.AddAsync(lote, cancellationToken);

    public Task AtualizarAsync(LoteEventos lote, CancellationToken cancellationToken = default)
    {
        _context.LotesEventos.Update(lote);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
