using ESocial.Domain.Entities;
using ESocial.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESocial.Infrastructure.Persistence.Configurations;

public class LoteEventosConfiguration : IEntityTypeConfiguration<LoteEventos>
{
    public void Configure(EntityTypeBuilder<LoteEventos> builder)
    {
        builder.ToTable("lotes_eventos");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(l => l.EmpregadorId).HasColumnName("empregador_id").IsRequired();
        builder.Property(l => l.NumeroLote).HasColumnName("numero_lote").IsRequired();

        builder.Property(l => l.Grupo)
            .HasColumnName("grupo_evento")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(l => l.Ambiente)
            .HasColumnName("ambiente")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(l => l.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(l => l.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(l => l.EnviadoEm).HasColumnName("enviado_em");
        builder.Property(l => l.ProcessadoEm).HasColumnName("processado_em");

        builder.OwnsOne(l => l.Protocolo, protocolo =>
        {
            protocolo.Property(p => p.Valor)
                .HasColumnName("protocolo")
                .HasMaxLength(60);
        });

        builder.OwnsOne(l => l.StatusProcessamento, sp =>
        {
            sp.Property(s => s.CdResposta).HasColumnName("cd_resposta").HasMaxLength(10);
            sp.Property(s => s.DescResposta).HasColumnName("desc_resposta").HasMaxLength(500);
        });

        builder.HasMany(l => l.Eventos)
            .WithOne()
            .HasForeignKey("lote_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(l => l.DomainEvents);

        builder.HasIndex(l => l.EmpregadorId);
        builder.HasIndex("Protocolo_Valor");
    }
}
