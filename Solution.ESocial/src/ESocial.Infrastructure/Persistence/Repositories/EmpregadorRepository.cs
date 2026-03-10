using ESocial.Domain.Entities;
using ESocial.Domain.Repositories;
using ESocial.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ESocial.Infrastructure.Persistence.Repositories;

public class EmpregadorRepository : IEmpregadorRepository
{
    private readonly ESocialDbContext _context;

    public EmpregadorRepository(ESocialDbContext context)
    {
        _context = context;
    }

    public async Task<Empregador?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Empregadores.FindAsync([id], cancellationToken);

    public async Task<Empregador?> ObterPorInscricaoAsync(Inscricao inscricao, CancellationToken cancellationToken = default)
        => await _context.Empregadores
            .FirstOrDefaultAsync(e =>
                e.Inscricao.Tipo == inscricao.Tipo &&
                e.Inscricao.Numero == inscricao.Numero,
                cancellationToken);

    public async Task AdicionarAsync(Empregador empregador, CancellationToken cancellationToken = default)
        => await _context.Empregadores.AddAsync(empregador, cancellationToken);

    public Task AtualizarAsync(Empregador empregador, CancellationToken cancellationToken = default)
    {
        _context.Empregadores.Update(empregador);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
