using ESocial.Domain.Entities;
using ESocial.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ESocial.Infrastructure.Persistence;

public class ESocialDbContext : DbContext
{
    public DbSet<Empregador> Empregadores => Set<Empregador>();
    public DbSet<LoteEventos> LotesEventos => Set<LoteEventos>();
    public DbSet<Evento> Eventos => Set<Evento>();

    public ESocialDbContext(DbContextOptions<ESocialDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EmpregadorConfiguration());
        modelBuilder.ApplyConfiguration(new LoteEventosConfiguration());
        modelBuilder.ApplyConfiguration(new EventoConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
